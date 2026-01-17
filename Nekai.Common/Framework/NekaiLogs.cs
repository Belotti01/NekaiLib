using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Serilog;
using Serilog.Core;
using ILogger = Serilog.ILogger;

namespace Nekai.Common;

/// <summary>
/// Container with factory methods for thread-safe <see cref="ILogger"/> instances.
/// </summary>
public static class NekaiLogs
{
	private static readonly Lock _lock = new();

	/// <summary>
	/// Factory used to generate the <see cref="NekaiLogs"/>' <see cref="ILogger"/> instances.
	/// </summary>
	public static NekaiLoggerFactory Factory { get; } = new();

	/// <summary>
	/// Thread-safe instance of <see cref="ILogger"/> that serializes logs into <see cref="NekaiData.Directories.SharedLogs"/>.
	/// </summary>
	/// <remarks>
	/// This is used internally by the Nekai libraries. For other programs it is recommended to use the <see cref="Program"/> logger instead.
	/// </remarks>
	public static ILogger Shared
	{
		get
		{
			if(field is not null)
				return field;

			string? sharedLogFilesTemplate = Path.Combine(NekaiData.Directories.SharedLogs, "Logs.json");
			var sharedLogPath = PathString.Parse(sharedLogFilesTemplate);
			var fileCreationResult = sharedLogPath.EnsureExistsAsFile();
			if(!fileCreationResult.IsSuccessful())
			{
				// Don't export to file.
				sharedLogFilesTemplate = null;
			}

			lock(_lock)
			{
				if(!_TryInstantiateGlobalLogger(ref field, sharedLogFilesTemplate))
					// Logger creation failed. Return a void logger, and retry on the next call.
					return Logger.None;

				if(sharedLogFilesTemplate is null)
				{
					field.Warning($"Logs folder could not be accessed or created. Logs will not be exported. (Error: {fileCreationResult.GetMessage()})");
					Debug.Assert(!fileCreationResult.IsSuccessful(), "Logical error: the file was found.");
				}
			}

			return field;
		}
	}

	/// <summary>
	/// Thread-safe instance of <see cref="ILogger"/> scoped to the currently running program. Serializes logs into
	/// <see cref="NekaiData.Directories.CurrentProgramLogs"/>.
	/// </summary>
	public static ILogger Program
	{
		get
		{
			if(field is not null)
				return field;

			string sharedLogFilesTemplate = Path.Combine(NekaiData.Directories.CurrentProgramLogs, "Logs.json");
			lock(_lock)
			{
				if(!_TryInstantiateGlobalLogger(ref field, sharedLogFilesTemplate))
					// Logger creation failed. Return a void logger, and retry on the next call.
					return Logger.None;
			}
			return field;
		}
	}

	/// <summary>
	/// Thread-safe instance of <see cref="ILogger"/> that writes exclusively to the Console.
	/// </summary>
	public static ILogger Console
	{
		get
		{
			if(field is not null)
				return field;

			lock(_lock)
			{
				if(!_TryInstantiateGlobalLogger(ref field, null))
					// Logger creation failed. Return a void logger, and retry on the next call.
					return Logger.None;
			}
			return field;
		}
	}

	private static bool _TryInstantiateGlobalLogger([NotNullWhen(true)] ref ILogger? logger, string? logFilePathTemplate)
	{
		// Logger might have been created while this thread was stuck due to the lock, so check beforehand.
		if(logger is not null)
			return true;

		try
		{
			NekaiLoggerConfiguration config = new()
			{
				LogToConsole = true
			};

			if(logFilePathTemplate is not null)
			{
				config.WithCompactJsonFileOutput(logFilePathTemplate);
			}
			var result = Factory.TryCreate(config);
			if(result.IsSuccessful)
			{
				logger = result.Value; 
				return true;
			}
		}
		catch(Exception ex)
		{
			Exceptor.ThrowAndLogIfDebug(ex);
		}
		return false;
	}

	public static List<NekaiLog> DeserializeAll(PathString sourceDirectory, bool recursive = false)
	{
		List<PathString> files = sourceDirectory.EnumerateFileSystemEntries()
			.Select(x => PathString.Parse(x))
			.ToList();

		List<NekaiLog> logs = [];

		foreach(PathString file in files)
		{
			if(file.IsExistingDirectory())
			{
				if(!recursive)
					continue;
				logs.AddRange(DeserializeAll(file));
				continue;
			}
			
			logs.AddRange(Deserialize(file));
		}

		return new(logs);
	}

	public static NekaiLog[] Deserialize(PathString filePath)
	{
		// The runtime-tied file can't be accessed without the FileShare.ReadWrite option.

        List<string> json = [];
		using var stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
		var line = stream.ReadLine();
		while(line is not null)
		{
			json.Add(line);
			line = stream.ReadLine();
		}

		NekaiLog[] fileLogs = json
			.Select(x => JsonSerializer.Deserialize<NekaiLog>(x))
			.ExceptNulls()
			.ToArray();
		return fileLogs;
	}
}
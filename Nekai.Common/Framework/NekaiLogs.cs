using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Serilog;
using Serilog.Core;

namespace Nekai.Common;

/// <summary>
/// Container with factory methods for thread-safe <see cref="ILogger"/> instances.
/// </summary>
public static class NekaiLogs
{
	private static readonly object _lock = new();
	private static ILogger? _sharedLogger;
	private static ILogger? _currentProgramLogger;

	/// <summary>
	/// Factory used to generate the <see cref="NekaiLogs"/>' <see cref="ILogger"/> instances.
	/// </summary>
	public static NekaiLoggerFactory Factory { get; } = new();

	/// <summary>
	/// Thread-safe instance of <see cref="ILogger"/> that serializes logs into <see cref="NekaiData.Directories.SharedLogs"/>.
	/// </summary>
	public static ILogger Shared
	{
		get
		{
			if(_sharedLogger is not null)
				return _sharedLogger;

			string? sharedLogFilesTemplate = Path.Combine(NekaiData.Directories.SharedLogs, "Logs.json");
			var sharedLogPath = PathString.Parse(sharedLogFilesTemplate);
			var fileCreationResult = sharedLogPath.EnsureExistsAsFile();
			if(!fileCreationResult.IsSuccess())
			{
				// Don't export to file.
				sharedLogFilesTemplate = null;
			}

			lock(_lock)
			{
				if(!_TryInstantiateGlobalLogger(ref _sharedLogger, sharedLogFilesTemplate))
					// Logger creation failed. Return a void logger, and retry on the next call.
					return Logger.None;

				if(sharedLogFilesTemplate is null)
				{
					_sharedLogger.Warning($"Logs folder could not be accessed or created. Logs will not be exported. (Error: {fileCreationResult.GetMessage()})");
					Debug.Assert(!fileCreationResult.IsSuccess(), "Logical error: the file was found.");
				}
			}

			return _sharedLogger;
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
			if(_currentProgramLogger is not null)
				return _currentProgramLogger;

			string sharedLogFilesTemplate = Path.Combine(NekaiData.Directories.CurrentProgramLogs, "Logs.json");
			lock(_lock)
			{
				if(!_TryInstantiateGlobalLogger(ref _currentProgramLogger, sharedLogFilesTemplate))
					// Logger creation failed. Return a void logger, and retry on the next call.
					return Logger.None;
			}
			return _currentProgramLogger;
		}
	}

	private static bool _TryInstantiateGlobalLogger([NotNullWhen(true)] ref ILogger? logger, string? logFilePathTemplate)
	{
		// Logger might have been created while this thread was stuck due to the lock, so check beforehand.
		if(logger is not null)
			return true;

		try
		{
			// TODO: Pick between the JSONFormatter and the outputTemplate for the Shared and Program loggers.
			// - Using the Json format will require another way to view the logs properly, but it is more software-friendly
			// - ... otherwise just make the logs straightforward with a simple prefix, but losing information in the process
			// fml idk keep it json for now
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

	public static IEnumerable<Log> DeserializeAll(string sourceDirectory, bool recursive = false)
	{
		List<PathString> files = Directory.EnumerateFileSystemEntries(sourceDirectory)
			.Select(x => PathString.Parse(x))
			.ToList();

		List<Log> logs = [];

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

		return logs;
	}

	public static IEnumerable<Log> Deserialize(string filePath)
	{
		var fileLogs = JsonSerializer.Deserialize<Log[]>(filePath);
		return fileLogs ?? Array.Empty<Log>();
	}
}
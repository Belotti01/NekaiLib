using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using Nekai.Common.Reflection;

namespace Nekai.Common;

/// <summary>
/// Base class for file-serializable objects.
/// </summary>
/// <remarks>
/// Use <see cref="JsonSerializableAttribute"/> in derived Types to improve performance.
/// </remarks>
public abstract class ConfigurationFileManager<TSelf>
where TSelf : ConfigurationFileManager<TSelf>
{
	/// <summary>
	/// Whether to also serialize and deserialize fields.
	/// </summary>
	[JsonIgnore]
	protected bool includeFields = false;
	/// <summary>
	/// The path to the file linked to this instance.
	/// </summary>
	[JsonIgnore]
	public virtual string? FilePath { get; private set; }

	// JsonSerializerOptions are single-use, so use a generator method rather than a static instance.
	private static JsonSerializerOptions _CreateSerializerOptions(bool includeFields)
		=> new(JsonSerializerDefaults.General)
		{
			WriteIndented = true,
			PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
			AllowTrailingCommas = true,
			IgnoreReadOnlyFields = false,
			IgnoreReadOnlyProperties = false,
			IncludeFields = includeFields
		};


	static ConfigurationFileManager()
	{
		Debug.Assert(typeof(TSelf).TryGetAttribute<JsonSerializableAttribute>(out _), $"Types inheriting {nameof(ConfigurationFileManager<TSelf>)} should be decorated with the {nameof(JsonSerializableAttribute)}.");
	}

	protected ConfigurationFileManager(string? filePath = null, bool includeFields = false)
	{
		this.includeFields = includeFields;
		_TrySetFilePath(filePath);
	}

	private Result _TrySetFilePath(string? filePath)
	{
		if(string.IsNullOrWhiteSpace(filePath))
			return Result.Failure("No filepath was specified.");

		// Don't append an extension to the path to avoid confusion when trying to access the file with the same string.

		Result result = NekaiFile.TryEnsureExists(filePath);
		if(!result.IsSuccess)
			return Result.Failure($"Configuration file could not be created or accessed. Serialization will be disabled: {result.Message}");

		result = NekaiFile.CanReadFile(filePath);
		if(!result.IsSuccess)
			return Result.Failure($"Configuration file cannot be accessed. Serialization will be disabled: {result.Message}");

		FilePath = filePath;
		return Result.Success();
	}



	public static Result<TSelf> Deserialize(string filePath)
	{
		var result = NekaiFile.TryReadText(filePath);
		if(!result.IsSuccess)
			return Result.Failure(result.Message);

		TSelf? obj;
		try
		{
			// Always include fields during deserialization. The choice of whether to include them is supposed to have an
			// effect during serialization, so if they're present in the serialized data, read them.
			obj = JsonSerializer.Deserialize<TSelf>(result.Value, _CreateSerializerOptions(true));
		}
		catch(Exception ex)
		{
			// Log a meaningful message, but return a more user-friendly one.
			NekaiLogs.Shared.Warning($"Deserialization of {typeof(TSelf).Name} \"{filePath}\" failed: {ex.Message}");
			return Result.Failure($"Could not load the requested data.");
		}

		if(obj is null)
			return Result.Failure($"The requested data could not be parsed.");

		obj.FilePath = filePath;
		return Result.Success(obj);

	}

	public static Result<TSelf> TryDeserialize(string filePath)
	{
		try
		{
			Result<TSelf> result = Deserialize(filePath);
			if(!result.IsSuccess)
				return Result.Failure(result.Message);
			result.Value.FilePath = filePath;
			return result;
		}
		catch(Exception ex)
		{
			NekaiLogs.Program.Error(ex);
			return Result.Failure($"An error occurred and the data could not be loaded.");
		}
	}



	public Result TrySerialize()
	{
		Result<string> result = NekaiPath.ValidatePath(FilePath);
		if(!result.IsSuccess)
			return Result.Failure(result.Message);

		FilePath = result.Value;
		using FileBackupManager backupManager = new(FilePath);
		if(File.Exists(FilePath))
		{
			Result<string> backupResult = backupManager.TryBackup();
			if(!result.IsSuccess)
				return Result.Failure($"{result.Message} Current instance will not be saved.");
		}
		else
		{
			result = NekaiDirectory.TryEnsureExistsForFile(FilePath);
			if(!result.IsSuccess)
				return Result.Failure($"Could not find or create a required directory. Current instance will not be saved.");
		}

		try
		{
			using FileStream stream = File.Create(FilePath);
			JsonSerializer.Serialize(stream, (TSelf)this, _CreateSerializerOptions(includeFields));
		}
		catch(Exception ex)
		{
			Result restoreResult = backupManager.TryRestore();
			if(!restoreResult.IsSuccess)
			{
				Exceptor.ThrowIfDebug($"Bruh everything went wrong here ({ex.Message})");
				return Result.Failure($"{restoreResult.Message} Some data may be lost.");
			}
		}

		return Result.Success();
	}

	/// <summary>
	/// Returns the JSON representation of this instance.
	/// </summary>
	public override string ToString()
	{
		return JsonSerializer.Serialize(this, _CreateSerializerOptions(includeFields));
	}
}
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
	public virtual PathString? FilePath { get; private set; }

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

	private PathOperationResult _TrySetFilePath(string? filePath)
	{
		var result = PathString.TryParse(filePath);
		if(!result.IsSuccessful)
			return result.Error;

		FilePath = result.Value;
		// Don't append an extension to the path to avoid confusion when trying to access the file with the same string.

		if(!FilePath.IsExistingFile())
			return PathOperationResult.DoesNotExist;

		if(!FilePath.CanBeReadAsFile())
			return PathOperationResult.FailedRead;

		return PathOperationResult.Success;
	}

	private static Result<TSelf, PathOperationResult> _DeserializeInternal(string filePath)
	{
		var result = NekaiFile.TryReadText(filePath);
		if(!result.IsSuccessful)
			return new(result.Error);

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
			return new(PathOperationResult.NotAllowed);
		}

		if(obj is null)
			return new(PathOperationResult.BadFormat);

		obj.FilePath = PathString.Parse(filePath);
		return obj;
	}

	public static Result<TSelf, PathOperationResult> TryDeserialize(string filePath)
	{
		try
		{
			var result = _DeserializeInternal(filePath);
			if(!result.IsSuccessful)
				return result;

			result.Value.FilePath = PathString.Parse(filePath);
			return result;
		}
		catch(Exception ex)
		{
			NekaiLogs.Program.Error(ex);
			return new(PathOperationResult.UnknownFailure);
		}
	}

	public PathOperationResult TrySerialize()
	{
		using FileBackupManager backupManager = new(FilePath);
		if(FilePath.IsExistingFile())
		{
			var backupResult = backupManager.TryBackup();
			if(!backupResult.IsSuccessful)
				return backupResult.Error;
		}
		else
		{
			var fileCreationResult = FilePath.EnsureExistsAsFile();
			if(!fileCreationResult.IsSuccess())
				return fileCreationResult;
		}

		try
		{
			using FileStream stream = File.Create(FilePath);
			JsonSerializer.Serialize(stream, (TSelf)this, _CreateSerializerOptions(includeFields));
		}
		catch(Exception ex)
		{
			var restoreResult = backupManager.TryRestore();
			if(!restoreResult.IsSuccess())
			{
				Exceptor.ThrowIfDebug($"Bruh everything went wrong here ({ex.Message})");
				return restoreResult;
			}
		}

		return PathOperationResult.Success;
	}

	/// <summary>
	/// Returns the JSON representation of this instance.
	/// </summary>
	public override string ToString()
	{
		return JsonSerializer.Serialize(this, _CreateSerializerOptions(includeFields));
	}
}
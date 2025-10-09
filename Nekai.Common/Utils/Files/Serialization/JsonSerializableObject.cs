using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Nekai.Common.Reflection;

namespace Nekai.Common;

/// <summary>
/// Base class for file-serializable objects.
/// </summary>
/// <remarks>
/// Use <see cref="JsonSerializableAttribute"/> in derived Types to improve performance.
/// </remarks>
public abstract class JsonSerializableObject<TSelf> : JsonSerializerContext
where TSelf : JsonSerializableObject<TSelf>
{
	[JsonPropertyName("LastSerialization")]
	public DateTime? LastSerialization { get; set; }
	
	/// <summary>
	/// The path to the file linked to this instance.
	/// </summary>
	[JsonIgnore]
	public virtual PathString? FilePath { get; private set; }

    // The default serializer options.
    [JsonIgnore]
    protected override JsonSerializerOptions? GeneratedSerializerOptions => 
		new(JsonSerializerDefaults.General)
		{
			IgnoreReadOnlyProperties = true,
			IgnoreReadOnlyFields = true,
			WriteIndented = true
		};
    
    [JsonIgnore]
    public new JsonSerializerOptions Options { get => base.Options; }
    
	static JsonSerializableObject()
	{
		Debug.Assert(typeof(TSelf).TryGetAttribute<JsonSerializableAttribute>(out _), $"Types inheriting {nameof(JsonSerializableObject<TSelf>)} should be decorated with the {nameof(JsonSerializableAttribute)}.");
	}

	protected JsonSerializableObject(PathString? filePath = null, JsonSerializerOptions? options = null)
		: base(options)
	{
		if(filePath is null)
			return;

		var result = _TrySetFilePath(filePath);
		if(!result.IsSuccessful())
		{
			NekaiLogs.Shared.Error("Couldn't set file '{path}' for serialization: {message}", filePath, result.GetMessage());
			Debug.Fail("Couldn't set filepath for serialization.");
        }
	}

	private PathOperationResult _TrySetFilePath(PathString filePath)
	{
		// Don't append an extension to the path to avoid confusion when trying to access the file with the same string.
		FilePath = filePath;
		
		var result = FilePath.EnsureExistsAsFile();
		if(!result.IsSuccessful())
			return result;
		
		if(!FilePath.IsExistingFile())
			return PathOperationResult.DoesNotExist;

		if(!FilePath.CanBeReadAsFile())
			return PathOperationResult.FailedRead;

		return PathOperationResult.Success;
	}

	private static Result<TSelf, PathOperationResult> _DeserializeInternal(PathString filePath, JsonSerializerOptions? options)
	{
		if(!filePath.CanBeReadAsFile())
			return new(PathOperationResult.FailedRead);

		var content = filePath.ReadFileContent();
		if(content is null)
			return new(PathOperationResult.FailedRead);

		TSelf? obj;
		try
		{
			obj = JsonSerializer.Deserialize<TSelf>(content, options);
		}
		catch(Exception ex)
		{
			NekaiLogs.Shared.Warning($"Deserialization of {typeof(TSelf).Name} \"{filePath}\" failed: {ex.GetFullMessage()}");
			return new(PathOperationResult.NotAllowed);
        }

        if(obj is null)
			return new(PathOperationResult.BadFormat);

		obj.FilePath = PathString.Parse(filePath);
		return obj;
	}

	public static Result<TSelf, PathOperationResult> TryDeserialize(string filePath, JsonSerializerOptions? options = null)
	{
		var result = PathString.TryParse(filePath);
		if(!result.IsSuccessful)
			return new(result.Error);

		return TryDeserialize(result.Value, options);
	}

	public static Result<TSelf, PathOperationResult> TryDeserialize(PathString filePath, JsonSerializerOptions? options = null)
	{
		var result = _DeserializeInternal(filePath, options);
		if(!result.IsSuccessful)
			return result;

		result.Value.FilePath = filePath;
		return result;
	}

	public static Result<TSelf> TryDeserializeJsonString(string json, JsonSerializerOptions? options = null)
	{
		TSelf? obj;
		try
		{
			obj = JsonSerializer.Deserialize<TSelf>(json, options);
		}
		catch(Exception ex)
		{
			NekaiLogs.Shared.Warning($"Deserialization of {typeof(TSelf).Name} failed: {ex.Message}");
			return new();
		}

		if(obj is null)
			return new();

		return obj;
	}

	public PathOperationResult TrySerialize()
	{
		if(FilePath is null)
			return PathOperationResult.PathIsEmpty;

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
			if(!fileCreationResult.IsSuccessful())
				return fileCreationResult;
		}

		try
		{
			using FileStream stream = File.Create(FilePath);
			LastSerialization = DateTime.Now;
			// Cast is necessary to include the derived class' properties.
			JsonSerializer.Serialize(stream, (TSelf)this, Options);
		}
		catch(Exception ex)
		{
			var restoreResult = backupManager.TryRestore();
			if(!restoreResult.IsSuccessful())
				Exceptor.ThrowIfDebug($"Multiple failures while serializing and restoring an instance of {typeof(TSelf).Name} ({ex.Message}).");
			return PathOperationResult.UnknownFailure;
		}

		return PathOperationResult.Success;
	}

	/// <summary>
	/// Returns the JSON representation of this instance.
	/// </summary>
	public override string ToString()
	{
		return JsonSerializer.Serialize((TSelf)this, Options);
	}
	
	/// <inheritdoc/>
	public override JsonTypeInfo? GetTypeInfo(Type type) => JsonTypeInfo.CreateJsonTypeInfo<TSelf>(Options);
}
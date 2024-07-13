using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Nekai.Common.Reflection;

namespace Nekai.Common;

/// <summary>
/// Base class for file-serializable objects.
/// </summary>
/// <remarks>
/// Use <see cref="JsonSerializableAttribute"/> in derived Types to improve performance.
/// </remarks>
public abstract class ConfigurationFileManager<TSelf> : JsonSerializerContext
where TSelf : ConfigurationFileManager<TSelf>
{
	/// <summary>
	/// The path to the file linked to this instance.
	/// </summary>
	[JsonIgnore]
	public virtual PathString? FilePath { get; private set; }

	// The default serializer options.
	protected override JsonSerializerOptions? GeneratedSerializerOptions => 
		new(JsonSerializerDefaults.General)
		{
			WriteIndented = true,
			PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
			AllowTrailingCommas = true,
			IgnoreReadOnlyFields = false,
			IgnoreReadOnlyProperties = false,
			IncludeFields = false
		};

	static ConfigurationFileManager()
	{
		Debug.Assert(typeof(TSelf).TryGetAttribute<JsonSerializableAttribute>(out _), $"Types inheriting {nameof(ConfigurationFileManager<TSelf>)} should be decorated with the {nameof(JsonSerializableAttribute)}.");
	}

	protected ConfigurationFileManager(string? filePath = null, JsonSerializerOptions? options = null)
		: base(options)
	{
		_TrySetFilePath(filePath);
	}

	protected ConfigurationFileManager(JsonSerializerOptions? options) 
		: base(options)
	{
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

	private static Result<TSelf, PathOperationResult> _DeserializeInternal(PathString filePath, JsonSerializerOptions? options)
	{
		if(!filePath.CanBeReadAsFile())
			return new(PathOperationResult.FailedRead);

		var content = filePath.ReadFileContent();

		TSelf? obj;
		try
		{
			if(content is null)
				throw new NullReferenceException("File content is null.");
			// Always include fields during deserialization. The choice of whether to include them is supposed to have an
			// effect during serialization, so if they're present in the serialized data, read them.
			obj = JsonSerializer.Deserialize<TSelf>(content, options);
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

	public PathOperationResult TrySerialize()
	{
		if(FilePath is null)
			throw new NullReferenceException($"Serialization path is null.");

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
			JsonSerializer.Serialize(stream, (TSelf)this, Options);
		}
		catch(Exception ex)
		{
			var restoreResult = backupManager.TryRestore();
			if(!restoreResult.IsSuccess())
			{
				Exceptor.ThrowIfDebug($"Multiple failures while serializing and restoring an instance of {typeof(TSelf).Name} ({ex.Message}).");
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
		return JsonSerializer.Serialize(this, Options);
	}
	
	/// <inheritdoc/>
	public override JsonTypeInfo? GetTypeInfo(Type type) => JsonTypeInfo.CreateJsonTypeInfo<TSelf>(Options);
}
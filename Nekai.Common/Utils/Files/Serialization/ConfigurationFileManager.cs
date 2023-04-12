using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Nekai.Common.Interfaces;

namespace Nekai.Common;

/// <summary>
/// Base class for file-serializable objects.
/// </summary>
/// <remarks>
/// Use <see cref="JsonSerializableAttribute"/> in derived Types to improve performance.
/// </remarks>
public abstract class ConfigurationFileManager<TSelf> : IJsonSerializable<TSelf>
where TSelf : ConfigurationFileManager<TSelf>
{
    private static JsonSerializerOptions _SerializerOptions { get; } = new(JsonSerializerDefaults.General)
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        AllowTrailingCommas = true,
        IgnoreReadOnlyFields = false,
        IgnoreReadOnlyProperties = false,
        IncludeFields = false
    };

    /// <summary>
    /// The path to the file linked to this instance.
    /// </summary>
    [JsonIgnore]
    public virtual string? FilePath { get; private set; }



	protected ConfigurationFileManager(string? filePath = null)
	{
        // Add JsonConverters here
        // _SerializerOptions.Converters.Add(...);

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



	public static Result<TSelf> Deserialize(string filePath, JsonSerializerOptions? options = null)
    {
		var result = NekaiFile.TryReadText(filePath);
        if(!result.IsSuccess)
            return Result.FromResult(result);
		
        TSelf? obj;
		try
        {
            obj = JsonSerializer.Deserialize<TSelf>(result.Value, options ?? JsonSerializerOptions.Default);
        }catch(Exception ex)
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

    public static Result<TSelf> TryDeserialize(string filePath, JsonSerializerOptions? options = null)
    {
        try
        {
            Result<TSelf> result = Deserialize(filePath, options);
            if(!result.IsSuccess)
                return Result.FromResult(result);
            result.Value.FilePath = filePath;
            return result;
        }
        catch (Exception ex)
        {
            NekaiLogs.Program.Error(ex);
            return Result.Failure($"An error occurred and the data could not be loaded.");
        }
    }
	


	public Result TrySerialize()
    {
        Result<string> result = NekaiPath.ValidatePath(FilePath);
        if(!result.IsSuccess)
            return Result.FromResult(result);

        FilePath = result.Value;
        using FileBackupManager backupManager = new(FilePath);
        if (File.Exists(FilePath))
        {
			Result<string> backupResult = backupManager.TryBackup();
			if (!result.IsSuccess)
                return Result.Failure($"{result.Message}. Current instance will not be saved.");
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
            JsonSerializer.Serialize(stream, (TSelf)this, _SerializerOptions);
        } catch(Exception ex)
        {
            Result restoreResult = backupManager.TryRestore();
            if(!restoreResult.IsSuccess)
            {
                Exceptor.ThrowIfDebug($"Bruh everything went wrong here\n{ex.Message}");
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
        return JsonSerializer.Serialize(this, _SerializerOptions);
    }
}
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Security;
using Nekai.Common;

namespace Nekai.Common;

/// <summary>
/// <see cref="File"/> methods wrapped to properly handle or prevent exceptions, and create user-friendly error messages.
/// </summary>
public static class NekaiFile
{
	/// <summary>
	/// Check whether a file is set to read-only, or if the file is a system file.
	/// </summary>
	/// <param name="filePath"> The path to the file. </param>
	/// <returns></returns>
	public static Result<bool> IsReadOnlyOrSystem(string filePath)
	{
		Result result = CanReadFile(filePath);
		if(!result.IsSuccess)
			return Result<bool>.Failure(result.Message);
		
		var attr = File.GetAttributes(filePath);
		bool isReadOnlyOrSystem = attr.HasFlag(FileAttributes.ReadOnly) || attr.HasFlag(FileAttributes.System);
		return Result.Success(isReadOnlyOrSystem);
	}

	// Make sure that the FileStream.Dispose() is invoked before returning.
	public static Result TryCreateOrOverwrite([NotNullWhen(true)] string? filePath)
	{
		Result<FileStream> result = _TryCreateOrOverwrite(filePath);
		if(!result.IsSuccess)
			return Result.Failure(result.Message);
		result.Value.Dispose();
		return Result.Success();
	}

	// The FileStreams should never be kept open for longer than required, so block off this method and wrap it instead.
	// The caller can still open its own stream when needed, but let other processes and threads access it in the meantime.
	private static Result<FileStream> _TryCreateOrOverwrite([NotNullWhen(true)] string? filePath, bool requireStream = false)
	{
		var result = NekaiPath.ValidatePath(filePath);
		if(!result.IsSuccess)
			return Result.Failure(result.Message);
		filePath = result.Value;

		try
		{
			var creationResult = NekaiDirectory._TryEnsureExistsForFileInternal(filePath, false);
			if(creationResult.IsSuccess)
			{
				return requireStream
					? Result.Success(File.Open(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
					: Result.Success(File.Open(filePath, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite));
			}
			FileStream stream = File.Create(filePath);
			Debug.Assert(File.Exists(filePath), "File creation didn't throw an exception, but the file doesn't exist.");
			return Result.Success(stream);
		}
		catch(Exception ex)
		{
			Debug.Assert(!File.Exists(filePath), $"File creation threw {ex.GetType().Name}, but the file exists.");
			return Result.Failure(NekaiPath.GetMessageForException(ex, filePath));
		}
	}

	/// <summary>
	/// Check whether the file exists, and attempt to delete it if it does.
	/// </summary>
	/// <param name="filePath"> The path to the file to check or delete. </param>
	/// <param name="tryIgnoreReadOnly"> When <see langword="true"/>, attempt to remove or bypass the ReadOnly flag of the file if it's enabled. </param>
	/// <returns> <see langword="true"/> if the file does not exists or has been deleted; <see langword="false"/> otherwise. </returns>
	public static Result TryEnsureDoesNotExist([NotNullWhen(true)] string? filePath, bool tryIgnoreReadOnly = false)
	{
		if(!File.Exists(filePath))
			return Result.Success();

		// Too many checks to make - just try-catch and branch based on the Exception type
		// after a simple file access permission check.
		var fileInfoResult = TryGetFileInfo(filePath);
		if(!fileInfoResult.IsSuccess)
			return Result.Failure(fileInfoResult.Message);
		
		try
		{
			FileInfo file = fileInfoResult.Value;
			if(file.Attributes.HasFlag(FileAttributes.System))
				return Result.Failure($"Attempted to delete System file \"{file.FullName}\"");

			if(file.IsReadOnly)
			{
				if(!tryIgnoreReadOnly)
					return Result.Failure($"File \"{file.FullName}\" is read-only.");
				file.IsReadOnly = false;
			}

			file.Delete();
			Debug.Assert(!File.Exists(filePath), "File deletion didn't throw an exception, but the file still exists.");
			return Result.Success();
		}
		catch(Exception ex)
		{
			Debug.Assert(File.Exists(filePath), $"File deletion threw {ex.GetType().Name}, but the file seems to have been deleted.");
			return Result.Failure(NekaiPath.GetMessageForException(ex, filePath));
		}
	}

	public static Result<IEnumerable<string>> TryReadLines([NotNullWhen(true)] string filePath)
	{
		Result result = NekaiPath.IsValidPath(filePath);
		if(!result.IsSuccess)
			return result;

		if(!File.Exists(filePath))
			return Result.Failure($"File could not be found.");

		try
		{
			return Result.Success(File.ReadLines(filePath));
		}
		catch(Exception ex)
		{
			return Result.Failure(NekaiPath.GetMessageForException(ex, filePath));
		}
	}

	public static Result<string> TryReadText([NotNullWhen(true)] string? filepath)
	{
		Result result = NekaiPath.IsValidPath(filepath);
		if(!result.IsSuccess)
			return result;
		
		if(!File.Exists(filepath))
			return Result.Failure("File does not exist.");

		try
		{
			string text = File.ReadAllText(filepath);
			return Result.Success(text);
		}catch(Exception ex)
		{
			return Result.Failure(NekaiPath.GetMessageForException(ex, filepath));
		}
	}

	public static Result TryEnsureExists([NotNullWhen(true)] string? filepath)
	{
		Result result = NekaiPath.IsValidPath(filepath);
		if(!result.IsSuccess)
			return result;

		if(File.Exists(filepath))
			return Result.Success();

		Result creationResult = TryCreateOrOverwrite(filepath);
		return creationResult.IsSuccess
			? Result.Success()
			: Result.Failure(creationResult.Message);
	}

	public static Result<FileInfo> TryGetFileInfo([NotNullWhen(true)] string? filepath)
	{
		var result = NekaiPath.ValidatePath(filepath);
		if(!result.IsSuccess)
			return Result<FileInfo>.Failure(result.Message);
		filepath = result.Value;

		try
		{
			FileInfo fileInfo = new(filepath);
			return Result<FileInfo>.Success(fileInfo);
		}
		catch(Exception ex)
		{
			return Result<FileInfo>.Failure(NekaiPath.GetMessageForException(ex, filepath));
		}
	}

	public static Result CanReadFile([NotNullWhen(true)] string? filePath)
		=> _CanReadFileInternal(filePath, true);

	/// <summary>
	/// Internal alternative to <see cref="CanReadFile(string?)"/> that allows skipping over the path validation, to avoid
	/// repeating the same checks twice.
	/// </summary>
	/// <remarks>
	/// When running in DEBUG mode, the check will be asserted instead to alert misuses of this method during development.
	/// </remarks>
	internal static Result _CanReadFileInternal([NotNullWhen(true)] string? filePath, bool validatePath)
	{
		if(validatePath)
		{
			var validationResult = NekaiPath.ValidatePath(filePath);
			if(!validationResult.IsSuccess)
				return Result.FromResult(validationResult);
			filePath = validationResult.Value;
		}else
		{
			Debug.Assert(NekaiPath.ValidatePath(filePath).IsSuccess, $"Misuse of internal method: validate the path before calling {nameof(_CanReadFileInternal)} with parameter {nameof(validatePath)} set to false.");
		}

		if(!File.Exists(filePath))
			return Result.Failure($"File \"{filePath}\" does not exist.");
		
		try
		{
			using FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
			return Result.Success();
		}
		catch(Exception ex)
		{
			return Result.Failure(NekaiPath.GetMessageForException(ex, filePath));
		}
	}
}
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Nekai.Common;

/// <summary>
/// <see cref="File"/> methods wrapped to properly handle or prevent exceptions, and create user-friendly error messages.
/// </summary>
public static class NekaiFile
{
	/// <summary>
	/// Check whether the file exists, and attempt to delete it if it does.
	/// </summary>
	/// <param name="filePath"> The path to the file to check or delete. </param>
	/// <param name="tryIgnoreReadOnly"> When <see langword="true"/>, attempt to remove or bypass the ReadOnly flag of the file if it's enabled. </param>
	/// <returns> <see langword="true"/> if the file does not exists or has been deleted; <see langword="false"/> otherwise. </returns>
	public static PathOperationResult TryEnsureDoesNotExist([NotNullWhen(true)] string? filePath, bool tryIgnoreReadOnly = false)
	{
		if(!File.Exists(filePath))
			return PathOperationResult.Success;

		// Too many checks to make - just try-catch and branch based on the Exception type
		// after a simple file access permission check.
		var fileInfoResult = TryGetFileInfo(filePath);
		if(!fileInfoResult.IsSuccessful)
			return fileInfoResult.Error;

		try
		{
			FileInfo file = fileInfoResult.Value;
			if(file.Attributes.HasFlag(FileAttributes.System))
				return PathOperationResult.IsSystem;

			if(file.IsReadOnly)
			{
				if(!tryIgnoreReadOnly)
					return PathOperationResult.IsReadOnly;
				file.IsReadOnly = false;
			}

			file.Delete();
			Debug.Assert(!File.Exists(filePath), "File deletion didn't throw an exception, but the file still exists.");
			return PathOperationResult.Success;
		}
		catch(Exception ex)
		{
			Debug.Assert(File.Exists(filePath), $"File deletion threw {ex.GetType().Name}, but the file seems to have been deleted.");
			return NekaiPath.GetResultFromException(ex);
		}
	}

	/// <summary>
	/// Attempt to read the file at <paramref name="filePath"/> and return its contents as an array of <see langword="string"/>s.
	/// </summary>
	/// <param name="filePath"> The path to the file to read. </param>
	/// <returns> An <see cref="IEnumerable{T}"/> of the lines of the file if it exists and can be read, or a value defining the error
	/// if not. </returns>
	public static Result<IEnumerable<string>, PathOperationResult> TryReadLines([NotNullWhen(true)] string filePath)
	{
		var result = NekaiPath.IsValidPath(filePath);
		if(!result.IsSuccess())
			return new(result);

		if(!File.Exists(filePath))
			return new(PathOperationResult.DoesNotExist);

		try
		{
			return new(File.ReadLines(filePath));
		}
		catch(Exception ex)
		{
			return new(NekaiPath.GetResultFromException(ex));
		}
	}

	public static Result<string, PathOperationResult> TryReadText([NotNullWhen(true)] string? filePath)
	{
		var result = NekaiPath.IsValidPath(filePath);
		if(!result.IsSuccess())
			return new(result);

		if(!File.Exists(filePath))
			return new(PathOperationResult.DoesNotExist);

		try
		{
			return File.ReadAllText(filePath);
		}
		catch(Exception ex)
		{
			return new(NekaiPath.GetResultFromException(ex));
		}
	}

	public static Result<Memory<byte>, PathOperationResult> TryReadBytes([NotNullWhen(true)] string? filePath)
	{
		var result = NekaiPath.IsValidPath(filePath);
		if(!result.IsSuccess())
			return new(result);

		if(!File.Exists(filePath))
			return new(PathOperationResult.DoesNotExist);

		try
		{
			return File.ReadAllBytes(filePath).AsMemory();
		}
		catch(Exception ex)
		{
			return new(NekaiPath.GetResultFromException(ex));
		}
	}

	public static PathOperationResult TryEnsureExists([NotNullWhen(true)] string? filepath)
	{
		var result = PathString.TryParse(filepath);
		if(!result.IsSuccessful)
			return result.Error;

		return result.Value.EnsureExistsAsFile();
	}

	public static Result<FileInfo, PathOperationResult> TryGetFileInfo([NotNullWhen(true)] string? filepath)
	{
		var result = NekaiPath.ValidatePath(filepath);
		if(!result.IsSuccessful)
			return new(result.Error);
		filepath = result.Value;

		try
		{
			FileInfo fileInfo = new(filepath);
			return fileInfo;
		}
		catch(Exception ex)
		{
			return new(NekaiPath.GetResultFromException(ex));
		}
	}

	public static PathOperationResult CanReadFile([NotNullWhen(true)] string? filePath)
		=> _CanReadFileInternal(filePath, true);

	/// <summary>
	/// Check whether the last access made to the file has been made within the specified <paramref name="time"/> (inclusive).
	/// </summary>
	/// <param name="filePath"> The path to the file to check. </param>
	/// <param name="time"> The expected maximum time distance since the last access to the file (inclusive). </param>
	/// <returns> <see langword="true"/> if the file was last accessed after <paramref name="time"/> amount of time before
	/// the current time, or <see langword="false"/> otherwise. If the <paramref name="filePath"/> is not a valid path, the file
	/// cannot be found or an error occurs, returns an unsuccesful <see cref="PathOperationResult"/> instead. </returns>
	public static Result<bool, PathOperationResult> WasLastAccessedWithin(string filePath, TimeSpan time)
	{
		var result = PathString.TryParse(filePath);
		if(!result.IsSuccessful)
			return new(result.Error);

		if(!result.Value.IsExistingFile())
			return new(PathOperationResult.DoesNotExist);

		filePath = result.Value;

		DateTime lastAccessUTC;
		try
		{
			lastAccessUTC = File.GetLastAccessTimeUtc(filePath);
		}
		catch(Exception ex)
		{
			// Metadata access denied, or file was not accessible.
			return new(NekaiPath.GetResultFromException(ex));
		}

		bool wasLastAccessInRange = DateTime.UtcNow <= lastAccessUTC + time;
		return wasLastAccessInRange;
	}

	/// <summary>
	/// Internal alternative to <see cref="CanReadFile(string?)"/> that allows skipping over the path validation, to avoid
	/// repeating the same checks twice.
	/// </summary>
	/// <remarks>
	/// When running in DEBUG mode, path validation errors will be asserted instead to alert of misuses of this method during development.
	/// </remarks>
	internal static PathOperationResult _CanReadFileInternal([NotNullWhen(true)] string? filePath, bool validatePath)
	{
		if(validatePath)
		{
			var validationResult = NekaiPath.IsValidPath(filePath!);
			return validationResult;
		}
		else
		{
			Debug.Assert(NekaiPath.IsValidPath(filePath!).IsSuccess(), $"Misuse of internal method: validate the path before calling {nameof(_CanReadFileInternal)} with parameter {nameof(validatePath)} set to false.");
		}

		if(!File.Exists(filePath))
			return PathOperationResult.DoesNotExist;

		try
		{
			using FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
			return PathOperationResult.Success;
		}
		catch(Exception ex)
		{
			return NekaiPath.GetResultFromException(ex);
		}
	}
}
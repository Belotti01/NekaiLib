using System.Diagnostics.CodeAnalysis;
using System.Security;

namespace Nekai.Common;

public static class NekaiPath
{

	public static bool TryRemovePathStep(ReadOnlySpan<char> path, out ReadOnlySpan<char> trimmedPath)
	{
		if(path.IsEmpty)
		{
			trimmedPath = "";
			return false;
		}
		
		path = Path.TrimEndingDirectorySeparator(path);
		int dirSeparatorIndex = LastIndexOfDirectorySeparatorChar(path);

		if(dirSeparatorIndex <= 0)
		{
			trimmedPath = "";
			return false;
		}

		trimmedPath = path[..dirSeparatorIndex];
		return !trimmedPath.IsEmpty;
	}

	public static int LastIndexOfDirectorySeparatorChar(ReadOnlySpan<char> path)
	{
		for(int i = path.Length - 1; i >= 0; i--)
		{
			if(path[i] == Path.DirectorySeparatorChar || path[i] == Path.AltDirectorySeparatorChar)
				return i;
		}
		return -1;
	}

	public static Result ContainsInvalidPathChars(ReadOnlySpan<char> path)
	{
		ReadOnlySpan<char> invalidChars = Path.GetInvalidPathChars();
		Set<char>? foundInvalidChars = null;
		foreach(char c in path)
		{
			if(!char.IsLetter(c) && invalidChars.Contains(c))
			{
				foundInvalidChars ??= new();
				foundInvalidChars.Add(c);
			}
		}
		return foundInvalidChars is null
			? Result.Success()
			: Result.Failure($"Path contains invalid characters: {string.Join(' ', foundInvalidChars)}");
	}

	public static string RemoveInvalidPathChars(string path)
	{
		int[] errorIndexes = path.IndexesOfAny(Path.GetInvalidPathChars());
		switch(errorIndexes.Length)
		{
			case 0:
				return path;

			case 1:
				return path.Remove(errorIndexes[0], 1);

			default:
				string str = string.Create(path.Length - errorIndexes.Length, (path, errorIndexes), (span, state) =>
				{
					int errorIndex = 0;
					for(int i = 0; i < state.path.Length; i++)
					{
						if(errorIndex < state.errorIndexes.Length && i == state.errorIndexes[errorIndex])
						{
							errorIndex++;
							continue;
						}
						span[i - errorIndex] = state.path[i];
					}
				});
				return str;
		}
	}

	/// <summary>
	/// Ensure the validity of the <paramref name="filePath"/>, and return the absolute form of it.
	/// </summary>
	/// <param name="filePath"> The path to validate. </param>
	/// <returns> The absolute path equivalent to the <paramref name="filePath"/> parameter. </returns>
	/// <remarks>
	/// Use this over <see cref="IsValidPath(string?)"/> because:
	/// <list type="bullet">
	///		<item>The absolute path is more specific, hence less ambiguous and debug-friendly</item>
	///		<item>The returned path's nullability actually works (unlike the [NotNullWhenAttribute], since the return value is not a raw <see langword="bool"/>)</item>
	/// </list>
	/// </remarks>
	public static Result<string> ValidatePath([NotNullWhen(true)] string? filePath)
	{
		if(string.IsNullOrWhiteSpace(filePath))
			return Result.Failure("No filepath was specified.");

		Result result = ContainsInvalidPathChars(filePath);
		if(!result.IsSuccess)
			return result;

		try
		{
			string fullPath = Path.GetFullPath(filePath);
			return Result.Success(fullPath);
		} catch(Exception ex)
		{
			return Result.Failure(GetMessageForException(ex, filePath));
		}
	}

	/// <summary>
	/// Check the validity of the <paramref name="filePath"/>.
	/// </summary>
	/// <param name="filePath"> The path to validate. </param>
	public static Result IsValidPath([NotNullWhen(true)] string? filePath)
		=> Result.FromResult(ValidatePath(filePath));

	internal static void _WriteLogForExceptionInternal(Exception exception, string filepath)
	{
		string message = GetMessageForException(exception, filepath);
		NekaiLogs.Shared.Error(message);
		NekaiLogs.Shared.Error(exception);
	}


	public static string GetMessageForException(Exception exception, ReadOnlySpan<char> filepath, bool isDirectory = false)
	{
		string result;
		string subject = isDirectory ? "directory" : "file";
		// To avoid privacy issues in case the string ends up being displayed, avoid including the full path to the file outside testing.
#if DEBUG
		result = exception switch
		{
			SecurityException => $"Access to {subject} \"{filepath}\" has been denied for security reasons.",
			UnauthorizedAccessException => $"Interrupted unauthorized operation attempt on {subject} \"{filepath}\".",
			PathTooLongException => $"Full path length of {subject} \"{filepath}\" exceeds OS limit.",
			ArgumentException => $"\"{filepath}\" is not a valid {subject} path.",
			DirectoryNotFoundException => $"A part of the directory path \"{filepath}\" does not exist.",
			_ => $"An unexpected error occurred while accessing {subject} \"{filepath}\": {exception.Message}"
		};
#else
		result = exception switch
		{
			SecurityException => $"Access to {subject} has been denied for security reasons.",
			UnauthorizedAccessException => $"Unauthorized operation attempt on {subject} was blocked.",
			PathTooLongException => $"Full path length of {subject} exceeds OS limit.",
			ArgumentException => $"Invalid {subject} path.",
			DirectoryNotFoundException => $"A directory in the path is missing.",
			_ => $"An unexpected error occurred while accessing the {subject}."
		};
#endif
		
		if(exception.InnerException is not null)
		{
			result += Environment.NewLine + GetMessageForException(exception.InnerException, filepath);
		}
		return result;
	}
}
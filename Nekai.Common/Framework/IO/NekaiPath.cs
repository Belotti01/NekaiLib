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

	public static bool TryRemovePathStep(string path, [NotNullWhen(true)] out string? trimmedPath)
	{
		if(string.IsNullOrWhiteSpace(path))
		{
			trimmedPath = null;
			return false;
		}

		path = Path.TrimEndingDirectorySeparator(path);
		int dirSeparatorIndex = LastIndexOfDirectorySeparatorChar(path);

		if(dirSeparatorIndex <= 0)
		{
			trimmedPath = null;
			return false;
		}

		trimmedPath = path[..dirSeparatorIndex];
		return trimmedPath.Length != 0;
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

	public static bool ContainsInvalidPathChars(ReadOnlySpan<char> path)
	{
		ReadOnlySpan<char> invalidChars = Path.GetInvalidPathChars();
		foreach(char c in path)
		{
			if(!char.IsLetter(c) && invalidChars.Contains(c))
				return true;
		}
		return false;
	}

	/// <summary>
	/// Remove all invalid path characters from the <paramref name="str"/>.
	/// </summary>
	/// <param name="str"> The <see langword="string"/> to check and modify. </param>
	/// <returns> The original <paramref name="str"/> if no invalid path characters are found, or a new <see langword="string"/> derived from it
	/// with all invalid path characters removed otherwise. </returns>
	public static string RemoveInvalidPathChars(string str)
	{
		int[] errorIndexes = str.IndexesOfAny(Path.GetInvalidPathChars());
		switch(errorIndexes.Length)
		{
			case 0:
				return str;

			case 1:
				return str.Remove(errorIndexes[0], 1);

			default:
				string validPathString = string.Create(str.Length - errorIndexes.Length, (str, errorIndexes), (span, state) =>
				{
					int errorIndex = 0;
					for(int i = 0; i < state.str.Length; i++)
					{
						if(errorIndex < state.errorIndexes.Length && i == state.errorIndexes[errorIndex])
						{
							errorIndex++;
							continue;
						}
						span[i - errorIndex] = state.str[i];
					}
				});
				return validPathString;
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
	public static Result<string, PathOperationResult> ValidatePath([NotNullWhen(true)] string? filePath)
	{
		// Make simple checks to avoid some unnecessary exception throws and report the proper error.
		if(string.IsNullOrWhiteSpace(filePath))
			return new(PathOperationResult.PathIsEmpty);

		if(ContainsInvalidPathChars(filePath) || !filePath.IsNormalized())
			return new(PathOperationResult.ContainsInvalidPathChars);

		try
		{
			return Path.GetFullPath(filePath);
		}
		catch(Exception ex)
		{
			return new(GetResultFromException(ex));
		}
	}

	/// <summary>
	/// Check the validity of the <paramref name="filePath"/>.
	/// </summary>
	/// <param name="filePath"> The path to validate. </param>
	public static PathOperationResult IsValidPath([NotNullWhen(true)] string filePath)
	{
		return ValidatePath(filePath).Error;
	}

	/// <summary>
	/// Return the <see cref="PathOperationResult"/> corresponding to the type of <paramref name="exception"/> thrown.
	/// </summary>
	/// <param name="exception"> The exception type to convert. </param>
	/// <returns> A <see cref="PathOperationResult"/> representing the thrown <paramref name="exception"/>. If a specific value can't be derived,
	/// this defaults to <see cref="PathOperationResult.UnknownFailure"/>. </returns>
	public static PathOperationResult GetResultFromException(Exception exception)
	{
		return exception switch
		{
			SecurityException => PathOperationResult.NotAllowed,
			UnauthorizedAccessException => PathOperationResult.NotAllowed,
			PathTooLongException => PathOperationResult.PathTooLong,
			ArgumentException => PathOperationResult.InvalidPath,
			DirectoryNotFoundException => PathOperationResult.DoesNotExist,
			_ => PathOperationResult.UnknownFailure
		};
	}
}
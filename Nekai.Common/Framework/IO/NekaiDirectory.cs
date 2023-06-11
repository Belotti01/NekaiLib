namespace Nekai.Common;

public static class NekaiDirectory
{

	public static PathOperationResult TryEnsureExists(string directory)
		=> _TryEnsureExistsInternal(directory, true);

	public static PathOperationResult TryEnsureExistsForFile(ReadOnlySpan<char> filePath)
		=> _TryEnsureExistsForFileInternal(filePath, true);

	// Overload used internally for when the path has already been validated
	internal static PathOperationResult _TryEnsureExistsInternal(string directory, bool checkInvalidChars)
	{
		if(string.IsNullOrWhiteSpace(directory))
			return PathOperationResult.PathIsEmpty;

		if(checkInvalidChars)
		{
			if(NekaiPath.ContainsInvalidPathChars(directory))
				return PathOperationResult.ContainsInvalidPathChars;
		}

		if(Directory.Exists(directory))
			return PathOperationResult.Success;

		try
		{
			Directory.CreateDirectory(directory);
			return PathOperationResult.Success;
		}
		catch(Exception ex)
		{
			return NekaiPath.GetResultFromException(ex);
		}
	}

	// Overload used internally for when the path has already been validated
	internal static PathOperationResult _TryEnsureExistsForFileInternal(ReadOnlySpan<char> filePath, bool checkInvalidChars)
	{
		ReadOnlySpan<char> directory;

		if(!NekaiPath.TryRemovePathStep(filePath, out directory))
		{
			// Path is relative, and the only information we have is the file name
			try
			{
				string absolutePath = Path.GetFullPath(filePath.ToString());
				if(!NekaiPath.TryRemovePathStep(filePath, out directory))
					return PathOperationResult.Success;    // At this point the path must be the root directory, so it must exist
			}
			catch(Exception ex)
			{
				return NekaiPath.GetResultFromException(ex);
			}
		}

		return _TryEnsureExistsInternal(directory.ToString(), checkInvalidChars);
	}
}
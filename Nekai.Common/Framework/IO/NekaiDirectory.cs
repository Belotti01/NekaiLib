namespace Nekai.Common;

public static class NekaiDirectory
{
	public static Result<DirectoryInfo> TryGetInfo(string directory)
	{
		if(string.IsNullOrWhiteSpace(directory))
			return Result.Failure("No directory was specified.");

		try
		{
			DirectoryInfo info = new(directory);
			return Result.Success(info);
		}
		catch(Exception ex)
		{
			return Result.Failure($"Directory information could not be retrieved: {NekaiPath.GetMessageForException(ex, directory, true)}");
		}
	}

	public static Result TryEnsureExists(string directory)
		=> _TryEnsureExistsInternal(directory, true);

	public static Result TryEnsureExistsForFile(ReadOnlySpan<char> filePath)
		=> _TryEnsureExistsForFileInternal(filePath, true);

	internal static void _EnsureExistsOrExitApplication(string directory)
	{
		if(TryEnsureExists(directory).IsSuccess)
			return;

		try
		{
			if(Directory.Exists(directory.ToString()))
				Exceptor.ThrowCritical(AppExitCode.DirectoryAccessError, $"Directory \"{directory}\" could not be accessed.");
			Exceptor.ThrowCritical(AppExitCode.DirectoryCreationError, $"Directory \"{directory}\" could not be created.");
		}
		catch(Exception ex)
		{
			string msg = NekaiPath.GetMessageForException(ex, directory, true);
			Exceptor.ThrowCritical(AppExitCode.DirectoryAccessError, msg, ex);
		}
	}

	// Overload used internally for when the path has already been validated
	internal static Result _TryEnsureExistsInternal(string directory, bool checkInvalidChars)
	{
		if(string.IsNullOrWhiteSpace(directory))
			return Result.Failure("Directory path is empty.");

		if(checkInvalidChars)
		{
			Result result = NekaiPath.ContainsInvalidPathChars(directory);
			if(!result.IsSuccess)
				return result;
		}

		if(Directory.Exists(directory.ToString()))
			return Result.Success();
		try
		{
			Directory.CreateDirectory(directory);
			return Result.Success();
		}
		catch(Exception ex)
		{
			return Result.Failure(NekaiPath.GetMessageForException(ex, directory, true));
		}
	}

	// Overload used internally for when the path has already been validated
	internal static Result _TryEnsureExistsForFileInternal(ReadOnlySpan<char> filePath, bool checkInvalidChars)
	{
		ReadOnlySpan<char> directory;

		if(!NekaiPath.TryRemovePathStep(filePath, out directory))
		{
			// Path is relative, and the only information we have is the file name
			try
			{
				string absolutePath = Path.GetFullPath(filePath.ToString());
				if(!NekaiPath.TryRemovePathStep(filePath, out directory))
					return Result.Success();    // At this point the path must be the root directory, so it must exist
			}
			catch(Exception ex)
			{
				return Result.Failure(NekaiPath.GetMessageForException(ex, directory, true));
			}
		}

		return _TryEnsureExistsInternal(directory.ToString(), checkInvalidChars);
	}
}
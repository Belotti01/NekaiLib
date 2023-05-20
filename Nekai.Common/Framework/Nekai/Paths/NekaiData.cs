namespace Nekai.Common;

public static partial class NekaiData
{
	/// <summary> To avoid deleting temporary files of which the lock has been temporarily released, but are still in use,
	/// keep them if they've been accessed recently. </summary>
	/// <remarks> Currently set to 3 minutes. </remarks>
	private static readonly TimeSpan _minimumTempFileAgeForDeletion = TimeSpan.FromMinutes(3);

	public static Result<int> ClearOldTempFiles()
	{
		string tempDirectory = Directories.Temp;
		if(!Directory.Exists(tempDirectory))
			return Result.Success(0);

		var filesEnumerator = Directory
			.EnumerateFiles(tempDirectory, "*", SearchOption.AllDirectories)
			.Where(filePath =>
			{
				// Only delete files that have been accessed recently.
				var result = NekaiFile.WasLastAccessedWithin(filePath, _minimumTempFileAgeForDeletion);
				return result.IsSuccess && !result.Value;
			});
		if(!filesEnumerator.Any())
			// Nothing to delete.
			return Result.Success(0);

		// The increments are blocking operations, but it can be useful information and it's (presumably) worth the performance hit.
		// Still better than pre-enumerating all files to get the count.
		int failedDeletions = 0;
		int deletions = 0;
		var operation = Parallel.ForEach(filesEnumerator, file =>
		{
			Result result = NekaiFile.TryEnsureDoesNotExist(file);
			if(!result.IsSuccess)
			{
				failedDeletions++;
			}
			else
			{
				deletions++;
			}
		});

		// Some files might be currently in use - only return an error if all deletions failed.
		if(deletions > 0)
			return Result.Success(deletions);

		return Result.Failure($"{failedDeletions} file{(failedDeletions > 1 ? "s" : "")} could not be removed.");
	}
}

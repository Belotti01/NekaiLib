﻿namespace Nekai.Common;

public static partial class NekaiData
{
	/// <inheritdoc cref="NekaiGeneralConfiguration"/>
	public static NekaiGeneralConfiguration Configuration => NekaiGeneralConfiguration.Singleton;
	
	/// <summary> To avoid deleting temporary files of which the lock has been temporarily released, but are still in use,
	/// keep them if they've been accessed recently. </summary>
	/// <remarks> Currently set to 3 minutes. </remarks>
	private static readonly TimeSpan _minimumTempFileAgeForDeletion = TimeSpan.FromMinutes(5);

	public static Result<int, PathOperationResult> ClearOldTempFiles()
	{
		string tempDirectory = Directories.Temp;
		if(!Directory.Exists(tempDirectory))
			return 0;

		var filesEnumerator = Directory
			.EnumerateFiles(tempDirectory, "*", SearchOption.AllDirectories)
			.Where(filePath =>
			{
				// Only delete files that have been accessed recently.
				var result = ((PathString)filePath!).WasLastAccessedWithin(_minimumTempFileAgeForDeletion);
				return result.IsSuccessful && !result.Value;
			});
		if(!filesEnumerator.Any())
			// Nothing to delete.
			return 0;

		// The increments are blocking operations, but it can be useful information and it's (presumably) worth the performance hit.
		// Still better than pre-enumerating all files to get the count.
		int failedDeletions = 0;
		int deletions = 0;
		var operation = Parallel.ForEach(filesEnumerator, file =>
		{
			var result = ((PathString)file)!.EnsureDeletion();
			if(!result.IsSuccessful())
			{
				failedDeletions++;
			}
			else
			{
				deletions++;
			}
		});

		// Some files might be currently in use - only return an error if ALL deletions failed.
		if(deletions > 0)
			return deletions;

		return new(PathOperationResult.UnknownFailure);
	}
}
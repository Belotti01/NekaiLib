using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Nekai.Common;

/// <summary>
/// Manager class used to safely create and restore file backups.
/// </summary>
public class FileBackupManager : IDisposable
{
	public string FilePath { get; protected set; }
	public string Filename => Path.GetFileName(FilePath);
	/// <summary> Whether a single backup file, if created, should be kept even after this instance is disposed. </summary>
	public bool KeepPersistentBackup { get; set; }

	public string BackupDirectory { get; protected set; }
	public string? BackupFilename { get; protected set; }

	public string? BackupFilePath => BackupFilename is null
		? null
		: Path.Combine(BackupDirectory, BackupFilename);

	/// <summary>
	/// Whether a backup file was created by this object.
	/// </summary>
	// File can be deleted at any time, so check every time rather than using a simple flag.
	[MemberNotNullWhen(true, nameof(BackupFilename), nameof(BackupFilePath))]
	public bool BackupExists => File.Exists(BackupFilePath);

	public FileBackupManager(string filepath)
	{
		BackupDirectory = Path.Combine(NekaiData.Directories.Temp, Environment.ProcessId.ToString());
		FilePath = filepath;
	}

	public FileBackupManager(string filepath, string backupDirectory)
	{
		if(!File.Exists(filepath))
			throw new FileNotFoundException($"File does not exist.", filepath);

		BackupDirectory = backupDirectory;
		FilePath = filepath;
	}

	public Result<string> TryBackup()
	{
		if(!File.Exists(FilePath))
			return Result.Failure("File to backup does not exist.");

		if(BackupExists)
		{
			Result result = NekaiFile.CanReadFile(BackupFilePath);
			if(!result.IsSuccess)
				return result;
		}

		try
		{
			return Result.Success(Backup());
		}
		catch(Exception ex)
		{
			return Result.Failure(NekaiPath.GetMessageForException(ex, BackupFilePath, false));
		}
	}

	public string Backup()
	{
		if(!File.Exists(FilePath))
			throw new FileNotFoundException();

		// If a backup file already exists, delete it, but only if the new backup file creation succeeds.
		string? oldBackupFilePath = BackupFilePath;

		BackupFilename = $"{DateTime.Now:yyyy-MM-dd_HH-mm-ss}_{Filename}.bak";
		Result backupFileCreationResult = NekaiFile.TryEnsureExists(BackupFilePath);
		if(!backupFileCreationResult.IsSuccess || BackupFilePath is null)   // Null check is just to shut the compiler up.
			throw new FileNotFoundException(backupFileCreationResult.Message, BackupFilePath);

		File.Copy(FilePath, BackupFilePath, true);

		if(oldBackupFilePath is not null)
		{
			// Delete the old backup file.
			Result duplicateDeletionResult = NekaiFile.TryEnsureDoesNotExist(BackupFilePath);
			if(!duplicateDeletionResult.IsSuccess)
			{
				Debug.Fail("Backup file could not be deleted: " + duplicateDeletionResult.Message);
				// Write a log entry, it might be useful to identify permission errors and such.
				NekaiLogs.Shared.Warning("Backup file could not be deleted: " + duplicateDeletionResult.Message);
				// Non-blocking error, but might result in useless disk usage. Continue anyway.
			}
		}
		return BackupFilePath;
	}

	public Result TryRestore()
	{
		if(!File.Exists(BackupFilePath))
			return Result.Failure("Backup file does not exist.");

		try
		{
			File.Copy(BackupFilePath, FilePath, true);
			// Assertion used to ensure that both files can be found (hence the <?? "weird string"> thing) and contain the same content.
			Debug.Assert(NekaiFile.CanReadFile(FilePath).IsSuccess, "Could not access restored file.");
			Debug.Assert(NekaiFile.TryReadText(FilePath).Value == NekaiFile.TryReadText(BackupFilePath).Value, "Content of source file differs from the restored one.");
			return Result.Success();
		}
		catch(Exception ex)
		{
			NekaiLogs.Shared.Error("Could not restore backup file:");
			NekaiLogs.Shared.Error(ex);

			return Result.Failure("Backup file could not be restored.");
		}
	}

	public void Dispose()
	{
		if(BackupFilePath is null || !File.Exists(BackupFilePath))
			return;

		if(!KeepPersistentBackup)
		{
			File.Delete(BackupFilePath);
		}
		BackupFilename = null;
		GC.SuppressFinalize(this);
	}
}
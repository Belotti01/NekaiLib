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

	public Result<string, PathOperationResult> TryBackup()
	{
		if(!File.Exists(FilePath))
			return new(PathOperationResult.DoesNotExist);

		if(BackupExists)
		{
			var result = NekaiFile.CanReadFile(BackupFilePath);
			if(!result.IsSuccess())
				return new(result);
		}

		try
		{
			return Backup();
		}
		catch(Exception ex)
		{
			return new(NekaiPath.GetResultFromException(ex));
		}
	}

	public string Backup()
	{
		if(!File.Exists(FilePath))
			throw new FileNotFoundException();

		// If a backup file already exists, delete it, but only if the new backup file creation succeeds.
		string? oldBackupFilePath = BackupFilePath;

		BackupFilename = $"{DateTime.Now:yyyy-MM-dd_HH-mm-ss}_{Filename}.bak";
		var backupFileCreationResult = NekaiFile.TryEnsureExists(BackupFilePath);
		if(!backupFileCreationResult.IsSuccess() || BackupFilePath is null)   // Null check is just to shut the compiler up.
			throw new FileNotFoundException(backupFileCreationResult.GetMessage(), BackupFilePath);

		File.Copy(FilePath, BackupFilePath, true);

		if(oldBackupFilePath is not null)
		{
			// Delete the old backup file.
			var duplicateDeletionResult = NekaiFile.TryEnsureDoesNotExist(BackupFilePath);
			if(!duplicateDeletionResult.IsSuccess())
			{
				Debug.Fail("Backup file could not be deleted: " + duplicateDeletionResult.GetMessage());
				// Write a log entry, it might be useful to identify permission errors and such.
				NekaiLogs.Shared.Warning("Backup file could not be deleted: " + duplicateDeletionResult.GetMessage());
				// Non-blocking error, but might result in useless disk usage. Continue anyway.
			}
		}
		return BackupFilePath;
	}

	public PathOperationResult TryRestore()
	{
		if(!File.Exists(BackupFilePath))
			return PathOperationResult.DoesNotExist;

		try
		{
			File.Copy(BackupFilePath, FilePath, true);
			// Assertion used to ensure that both files can be found and contain the same content.
			Debug.Assert(NekaiFile.CanReadFile(FilePath).IsSuccess(), "Could not access restored file.");
			Debug.Assert(NekaiFile.TryReadText(FilePath).Value == NekaiFile.TryReadText(BackupFilePath).Value, "Content of source file differs from the restored one.");
			return PathOperationResult.Success;
		}
		catch(Exception ex)
		{
			NekaiLogs.Shared.Error("Could not restore backup file:");
			NekaiLogs.Shared.Error(ex);

			return NekaiPath.GetResultFromException(ex);
		}
	}

	public void Dispose()
	{
		if(BackupFilePath is null || !File.Exists(BackupFilePath))
			return;

		if(!KeepPersistentBackup && File.Exists(BackupFilePath))
		{
			var result = NekaiFile.TryEnsureDoesNotExist(BackupFilePath);
			if(!result.IsSuccess())
			{
				NekaiLogs.Program.Warning($"Backup file could not be deleted upon disposal of object of type {GetType().Name}; {result.GetMessage()}");
			}
		}
		BackupFilename = null;
		GC.SuppressFinalize(this);
	}
}
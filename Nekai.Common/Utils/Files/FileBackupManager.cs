using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using CommunityToolkit.Diagnostics;

namespace Nekai.Common;

/// <summary>
/// Manager class used to safely create and restore file backups.
/// </summary>
public class FileBackupManager : IDisposable
{
	protected bool _Disposed { get; private set; } = false;
	
	public PathString FilePath { get; protected set; }
	public string FileName => Path.GetFileName(FilePath);

	/// <summary> Whether a backup file, if any is created, should be kept even after this instance is disposed. </summary>
	public bool KeepPersistentBackup { get; set; }

	public PathString BackupDirectory { get; protected set; }
	public string? BackupFileName { get; protected set; }

	[NotNullIfNotNull(nameof(BackupFileName))]
	public PathString? BackupFilePath => BackupFileName is null
		? null
		: PathString.Parse(Path.Combine(BackupDirectory, BackupFileName));

	/// <summary>
	/// Whether a backup file was created by this object.
	/// </summary>
	// File can be deleted at any time, so check every time rather than using a simple flag.
	[MemberNotNullWhen(true, nameof(BackupFileName), nameof(BackupFilePath))]
	public bool BackupExists => BackupFilePath?.IsExistingFile() ?? false;

	public FileBackupManager(string filepath)
	{
		FilePath = PathString.Parse(filepath);
		string backupDirectory = Path.Combine(NekaiData.Directories.Temp, Environment.ProcessId.ToString());
		BackupDirectory = PathString.Parse(backupDirectory);
		BackupDirectory.EnsureExistsAsDirectory();
	}

	public FileBackupManager(string filepath, string backupDirectory)
	{
		BackupDirectory = PathString.Parse(backupDirectory);
		FilePath = PathString.Parse(filepath);
	}

	public Result<PathString, PathOperationResult> TryBackup()
	{
		if(_Disposed)
			return new(PathOperationResult.ObjectDisposed);
		
		if(!File.Exists(FilePath))
			return new(PathOperationResult.DoesNotExist);

		if(BackupExists)
		{
			if(!BackupFilePath.CanBeReadAsFile())
				return new(PathOperationResult.FailedRead);
		}

		try
		{
			return Backup();
		}
		catch(Exception ex)
		{
			Debug.Fail("Backup operation failed even after checks.");
			NekaiLogs.Shared.Error("Failed backup operation for file '{path}' into '{backupPath}': {ex}", FilePath, BackupFilePath?.Path, ex.GetFullMessage());
			return new(NekaiPath.GetResultFromException(ex));
		}
	}

	public PathString Backup()
	{
		if(_Disposed)
			ThrowHelper.ThrowObjectDisposedException(nameof(FileBackupManager));

		var originalFileResult = PathString.TryParse(FilePath);
		if(!originalFileResult.IsSuccessful)
			throw new FormatException("File path is not a valid path.");
		if(!originalFileResult.Value.IsExistingFile())
			throw new FileNotFoundException("The file to backup does not exist.");

		// If a backup file already exists, delete it, but only if the new backup file creation succeeds.
		PathString? oldBackupFilePath = BackupFilePath;

		BackupFileName = $"{DateTime.Now:yyyy-MM-dd_HH-mm-ss}_{FileName}.bak";

		Debug.Assert(BackupFilePath is not null, $"{nameof(BackupFilePath)} is null even after assigning {nameof(BackupFileName)}.");
		Debug.Assert(!NekaiPath.ContainsInvalidPathChars(BackupFilePath), $"{nameof(BackupFilePath)} contains invalid path characters.");
		
		PathString backupFilePath = BackupFilePath!;
		File.Copy(FilePath, backupFilePath.Path, true);

		if(oldBackupFilePath is null)
			return backupFilePath;
		
		// Delete the old backup file.
		var duplicateDeletionResult = oldBackupFilePath.EnsureDeletion();
		if(duplicateDeletionResult.IsSuccessful())
			return backupFilePath;
		
		// Write a log entry, it might be useful to identify permission errors and such.
		NekaiLogs.Shared.Warning("Backup file could not be deleted: {message}", duplicateDeletionResult.GetMessage());
		return backupFilePath;
	}

	public PathOperationResult TryRestore()
	{
		if(_Disposed)
			return PathOperationResult.ObjectDisposed;
		
		if(!BackupExists)
			return PathOperationResult.DoesNotExist;

		try
		{
			File.Copy(BackupFilePath, FilePath, true);
			Debug.Assert(FilePath.CanBeReadAsFile(), "Could not access restored file.");
			return PathOperationResult.Success;
		}
		catch(Exception ex)
		{
			NekaiLogs.Shared.Error("Could not restore backup file: {ex}", ex.Message);

			return NekaiPath.GetResultFromException(ex);
		}
	}

	public void Dispose()
	{
		if(_Disposed)
			return;
		_Disposed = true;

		if(BackupFilePath is null || !BackupFilePath.IsExistingFile())
		{
			GC.SuppressFinalize(this);
			return;
		}

		if(!KeepPersistentBackup)
		{
			var result = BackupFilePath.EnsureDeletion();
			if(!result.IsSuccessful())
			{
				NekaiLogs.Program.Warning("Backup file could not be deleted upon disposal of object of type {type}; {message}", GetType().Name, result.GetMessage());
			}
		}
		BackupFileName = null;
		GC.SuppressFinalize(this);
	}
}
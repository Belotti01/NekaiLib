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
	public string Filename => Path.GetFileName(FilePath);

	/// <summary> Whether a backup file, if any is created, should be kept even after this instance is disposed. </summary>
	public bool KeepPersistentBackup { get; set; }

	public PathString BackupDirectory { get; protected set; }
	public string? BackupFilename { get; protected set; }

	[NotNullIfNotNull(nameof(BackupFilename))]
	public PathString? BackupFilePath => BackupFilename is null
		? null
		: PathString.Parse(Path.Combine(BackupDirectory, BackupFilename));

	/// <summary>
	/// Whether a backup file was created by this object.
	/// </summary>
	// File can be deleted at any time, so check every time rather than using a simple flag.
	[MemberNotNullWhen(true, nameof(BackupFilename), nameof(BackupFilePath))]
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
			NekaiLogs.Shared.Error("Failed backup operation for file '{path}' into '{backupPath}': {message}", FilePath, BackupFilePath?.Path, ex.GetFullMessage());
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

		BackupFilename = $"{DateTime.Now:yyyy-MM-dd_HH-mm-ss}_{Filename}.bak";
		PathString backupFilePath = BackupFilePath!;

		File.Copy(FilePath, backupFilePath.Path, true);

		if(oldBackupFilePath is null)
			return backupFilePath;
		
		// Delete the old backup file.
		var duplicateDeletionResult = oldBackupFilePath.EnsureDeletion();
		if(duplicateDeletionResult.IsSuccessful())
			return backupFilePath;
		
		// Write a log entry, it might be useful to identify permission errors and such.
		NekaiLogs.Shared.Warning("Backup file could not be deleted: " + duplicateDeletionResult.GetMessage());
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
			// Assertion used to ensure that both files can be found and contain the same content.
			Debug.Assert(FilePath.CanBeReadAsFile(), "Could not access restored file.");
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
		if(_Disposed)
			return;
		_Disposed = true;
		
		if(BackupFilePath is null || !BackupFilePath.IsExistingFile())
			return;

		if(!KeepPersistentBackup)
		{
			var result = BackupFilePath.EnsureDeletion();
			if(!result.IsSuccessful())
			{
				NekaiLogs.Program.Warning($"Backup file could not be deleted upon disposal of object of type {GetType().Name}; {result.GetMessage()}");
			}
		}
		BackupFilename = null;
		GC.SuppressFinalize(this);
	}
}
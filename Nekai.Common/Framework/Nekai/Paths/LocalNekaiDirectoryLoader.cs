namespace Nekai.Common;

// - Made standalone to avoid circular dependency, while keeping access to App.Directories and App.Files as simple
// as possible.
// - The Global configuration paths are kept private, since they are not supposed to be accessed for anything else.
public static partial class NekaiData
{
	/// <summary>
	/// Standalone class to read the global configuration file and fetch the local configuration directory path.
	/// </summary>
	internal static class _LocalNekaiDirectoryLoader
	{
		private static string _GlobalConfigurationDirectory => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Nekai");
		private static string _GlobalConfigurationFilepath => Path.Combine(_GlobalConfigurationDirectory, "GlobalConfiguration.json");
		private static string _DefaultLocalConfigurationDirectory => Path.Combine(_GlobalConfigurationDirectory, "Local");

		internal static string _Load()
		{
			string localDir = _GetLocalDirectoryPath();
			var result = NekaiDirectory.TryEnsureExists(localDir);
			if(!result.IsSuccess())
				Exceptor.ThrowCritical(AppExitCode.DirectoryCreationError, result.GetMessage());
			return localDir;
		}

		private static string _GetLocalDirectoryPath()
		{
			// It may be beneficial in the future to add checks of the last modified date of the files in <path> and
			// <_DefaultLocalConfigurationDirectory>, and overwrite the first if the second is newer.

			string? localDir = null;
			var fileReadResult = NekaiFile.TryReadText(_GlobalConfigurationFilepath);
			if(fileReadResult.IsSuccessful)
			{
				// Fetched from global config file - validate it
				localDir = fileReadResult.Value.Trim();
				if(!NekaiPath.IsValidPath(localDir).IsSuccess())
				{
					localDir = null;
				}
			}
			else
			{
				// Create file, and exit the app in case of file access errors (such as permission errors)
				var result = NekaiFile.TryEnsureExists(_GlobalConfigurationFilepath);
				if(!result.IsSuccess())
				{
					AppExitCode exitCode = File.Exists(_GlobalConfigurationFilepath)
						? AppExitCode.FileAccessError
						: AppExitCode.FileCreationError;
					Exceptor.ThrowCritical(exitCode, "Configuration could not be loaded. " + result.GetMessage());
				}
			}

			if(localDir is null)
			{
				localDir = _DefaultLocalConfigurationDirectory;
				File.WriteAllText(_GlobalConfigurationFilepath, _DefaultLocalConfigurationDirectory);
			}

			return localDir;
		}
	}
}
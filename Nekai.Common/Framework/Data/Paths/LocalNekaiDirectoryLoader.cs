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
			var localDir = _GetLocalDirectoryPath();
			bool exists = localDir?.EnsureExistsAsDirectory() == PathOperationResult.Success;
			if(!exists)
				Exceptor.ThrowCritical(AppExitCode.DirectoryCreationError);

			return localDir!;
		}

		private static PathOperationResult _CheckAndCreateDirectory(string dir, out PathString path)
		{
			path = null!;

			var parseResult = PathString.TryParse(dir);
			if(!parseResult.IsSuccessful)
				return parseResult.Error;
			path = parseResult.Value;

			var creationResult = path.EnsureExistsAsDirectory();
			return creationResult;
		}

		private static PathString? _GetLocalDirectoryPath()
		{
			// It may be beneficial in the future to add checks of the last modified date of the files in <path> and
			// <_DefaultLocalConfigurationDirectory>, and overwrite the first if the second is newer.

			// Ensure that the fixed filepath is valid
			var conversionResult = PathString.TryParse(_GlobalConfigurationFilepath);
			if(!conversionResult.IsSuccessful)
				Exceptor.ThrowCritical(AppExitCode.FixedPathError, conversionResult.Error.GetMessage());

			PathOperationResult result;
			PathString globalPath = conversionResult.Value, localPath;
			if(globalPath.IsExistingFile())
			{
				var rawLocalPath = globalPath.GetFileContent();
				// Fetched from global config file - validate it
				result = _CheckAndCreateDirectory(rawLocalPath.Trim(), out localPath);
				if(result != PathOperationResult.Success)
					return localPath;
			}

			// Create file, and exit the app in case of file access errors (such as permission errors)
			result = _CheckAndCreateDirectory(_GlobalConfigurationFilepath, out localPath);
			if(result == PathOperationResult.Success)
				return localPath;

			AppExitCode exitCode = File.Exists(_GlobalConfigurationFilepath)
				? AppExitCode.FileAccessError
				: AppExitCode.FileCreationError;
			Exceptor.ThrowCritical(exitCode, "Configuration could not be loaded: " + result.GetMessage());

			result = _CheckAndCreateDirectory(_DefaultLocalConfigurationDirectory, out localPath);
			if(result == PathOperationResult.Success)
				return localPath;

			try
			{
				File.WriteAllText(_GlobalConfigurationFilepath, _DefaultLocalConfigurationDirectory);
			}
			catch
			{
				// TODO: ... well sh1t
			}

			return localPath;
		}
	}
}
namespace Nekai.Common;

public static partial class NekaiData
{
	/// <summary>
	/// Contains the absolute paths to the directories used by the framework, automatically created upon access.
	/// </summary>
	public static class Directories
	{
		private static readonly Lazy<string> _localConfiguration = new(_LocalNekaiDirectoryLoader._Load);

		/// <summary> Framework configuration directory. </summary>
		public static PathString LocalConfiguration => _CreateLocalConfigDirectory();

		/// <summary> Folder to use to store temporary files. </summary>
		public static PathString Temp => _CreateDirectory(Path.GetTempPath(), "Nekai");

		/// <summary> Directory for UI personalization data. </summary>
		public static PathString UI => _CreateLocalConfigDirectory("UI");

		/// <summary> Folder to store color palettes into. </summary>
		public static PathString ColorPalettes { get; private set; } = _CreateDirectory(UI, "Colors");

		/// <summary> Folder to store themes into. </summary>
		public static PathString Themes => _CreateDirectory(UI, "Themes");

		/// <summary> Directory containing the log folders. </summary>
		public static PathString Logs => _CreateLocalConfigDirectory("Logs");

		/// <summary> Folder for log files shared across programs. </summary>
		public static PathString SharedLogs => _CreateDirectory(Logs, "Shared");

		/// <summary> Folder for log files scoped to single programs. </summary>
		public static PathString ProgramsLogs => _CreateDirectory(Logs, "Programs");

		/// <summary> Folder for log files scoped to the current program. </summary>
		public static PathString CurrentProgramLogs => _CreateDirectory(ProgramsLogs, NekaiPath.RemoveInvalidPathChars(NekaiApp.Name));

		/// <summary> View information of the currently active program. </summary>
		public static PathString CurrentProgramViews => _CreateProgramDirectory("Views");

		/// <summary> Storage for data specific to the currently active program. </summary>
		public static PathString CurrentProgramData => _CreateProgramDirectory("Data");

		/// <summary> Configurations folder of the current program. </summary>
		public static PathString CurrentProgramConfiguration => _CreateProgramDirectory("Configuration");



		private static PathString _CreateProgramDirectory(params string[] relativePath)
			=> _CreateDirectory(relativePath.Prepend<string>(_CreateLocalConfigDirectory("ProgramData", NekaiApp.Name)).ToArray());

		private static PathString _CreateLocalConfigDirectory(params string[] relativePath)
			=> _CreateDirectory(relativePath.Prepend(_localConfiguration.Value).ToArray());

		private static PathString _CreateDirectory(params string[] fullPath)
		{
			string pathString = Path.Combine(fullPath);
			var result = PathString.TryParse(pathString);

			if(!result.IsSuccessful)
			{
				throw new NekaiInternalException($"The constructed shared directory path is not valid: {string.Join(" + ", fullPath)}.", nameof(fullPath));
			}

			var path = result.Value;
			var creationResult = path.EnsureExistsAsDirectory();

			if(!creationResult.IsSuccessful())
			{
				throw new NekaiInternalException($"The shared directory could not be created (error: {creationResult}): {string.Join(" + ", fullPath)}.", nameof(fullPath));
			}

			return path;
		}
	}
}
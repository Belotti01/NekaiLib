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
		public static string LocalConfiguration => _CreateLocal();

		/// <summary> Folder to use to store temporary files. </summary>
		public static string Temp => _Create(Path.GetTempPath(), "Nekai");

		/// <summary> Directory for UI personalization data. </summary>
		public static string UI => _CreateLocal("UI");

		/// <summary> Folder to store color palettes into. </summary>
		public static string ColorPalettes { get; private set; } = _Create(UI, "Colors");

		/// <summary> Folder to store themes into. </summary>
		public static string Themes => _Create(UI, "Themes");

		/// <summary> Directory containing the log folders. </summary>
		public static string Logs => _CreateLocal("Logs");

		/// <summary> Folder for log files shared across programs. </summary>
		public static string SharedLogs => _Create(Logs, "Shared");

		/// <summary> Folder for log files scoped to single programs. </summary>
		public static string ProgramsLogs => _Create(Logs, "Programs");

		/// <summary> Folder for log files scoped to the current program. </summary>
		public static string CurrentProgramLogs => _Create(ProgramsLogs, NekaiPath.RemoveInvalidPathChars(NekaiApp.Name));

		/// <summary> View information of the currently active program. </summary>
		public static string CurrentProgramViews => _CreateProgram("Views");

		/// <summary> Storage for data specific to the currently active program. </summary>
		public static string CurrentProgramData => _CreateProgram("Data");

		/// <summary> Configurations folder of the current program. </summary>
		public static string CurrentProgramConfiguration => _CreateProgram("Configuration");



		private static PathString _CreateProgram(params string[] relativePath)
			=> _Create(relativePath.Prepend(_CreateLocal("ProgramData", NekaiApp.Name)).ToArray());

		private static PathString _CreateLocal(params string[] relativePath)
			=> _Create(relativePath.Prepend(_localConfiguration.Value).ToArray());

		private static PathString _Create(params string[] fullPath)
		{
			string pathString = Path.Combine(fullPath);
			var result = PathString.TryParse(pathString);

			if(!result.IsSuccessful)
			{
				throw new NekaiInternalException($"The constructed shared directory path is not valid: {string.Join(" + ", fullPath)}.", nameof(fullPath));
			}

			var path = result.Value;
			var creationResult = path.EnsureExistsAsDirectory();

			if(!creationResult.IsSuccess())
			{
				throw new NekaiInternalException($"The shared directory could not be created (error: {creationResult}): {string.Join(" + ", fullPath)}.", nameof(fullPath));
			}

			return path;
		}
	}
}
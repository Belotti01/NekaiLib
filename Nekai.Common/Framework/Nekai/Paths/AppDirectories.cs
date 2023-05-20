﻿namespace Nekai.Common;

public static partial class NekaiData
{
	/// <summary>
	/// Contains the absolute paths to the directories used by the framework.
	/// </summary>
	public static class Directories
	{
		private static readonly Lazy<string> _localConfiguration = new(_LocalNekaiDirectoryLoader._Load);
		/// <summary> Framework configuration directory. </summary>
		public static string LocalConfiguration => _Local();
		/// <summary> Folder to use to store temporary files. </summary>
		public static string Temp => Path.Combine(Path.GetTempPath(), "Nekai");
		/// <summary> Directory for UI personalization data. </summary>
		public static string UI => _Local("UI");
		/// <summary> Folder to store color palettes into. </summary>
		public static string ColorPalettes { get; private set; } = Path.Combine(UI, "Colors");
		/// <summary> Folder to store themes into. </summary>
		public static string Themes => Path.Combine(UI, "Themes");
		/// <summary> Directory containing the log folders. </summary>
		public static string Logs => _Local("Logs");
		/// <summary> Folder for log files shared across programs. </summary>
		public static string SharedLogs => Path.Combine(Logs, "Shared");
		/// <summary> Folder for log files scoped to single programs. </summary>
		public static string ProgramsLogs => Path.Combine(Logs, "Programs");
		/// <summary> Folder for log files scoped to the current program. </summary>
		public static string CurrentProgramLogs => Path.Combine(ProgramsLogs, NekaiPath.RemoveInvalidPathChars(CurrentApp.Name));

		// Help avoid errors at runtime due to missing folders
		static Directories()
		{
			ReadOnlySpan<string> paths = new string[]
			{
				LocalConfiguration,
				Temp,
				ColorPalettes,
				Themes,
				SharedLogs,
				ProgramsLogs,
				CurrentProgramLogs
			}.AsSpan();

			foreach(string path in paths)
			{
				Result result = NekaiDirectory.TryEnsureExists(path);
				if(!result.IsSuccess)
				{
					// Directory could not be found or created
					Exceptor.ThrowIfDebug(result.Message);
					// In release, attempt to continue execution.
					// Directories should always be checked for availability before access, since they can be deleted,
					// moved, renamed or locked at any time; so this is not to be treated as a critical error.

					// Note: Using the NekaiFile and NekaiDirectory wrapper classes instead of the .NET File and Directory when possible
					// helps minimize errors, while also enforcing proper error handling and standardize log formats for easier
					// bug report analisys.
				}
			}
		}


		private static string _Local(params string[] relativePath)
		{
			string pathStart = _localConfiguration.Value;
			string path = Path.Combine(relativePath.Prepend(pathStart).ToArray());
			return path;
		}
	}
}
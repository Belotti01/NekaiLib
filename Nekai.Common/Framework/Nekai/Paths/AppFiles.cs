namespace Nekai.Common;

public static partial class NekaiData
{
	/// <summary>
	/// Contains the absolute paths to the files used by the framework.
	/// </summary>
	public static class Files
	{
		public static string LogSettingsFile => Path.Combine(Directories.SharedLogs, "Logger_Settings.json");
		public static string GeneralSettingsFile => Path.Combine(Directories.LocalConfiguration, "Settings.json");
	}
}
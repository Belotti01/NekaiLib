namespace Nekai.Common;

public static partial class NekaiData
{
	public static class Files
	{
		public static string LogSettingsFile => Path.Combine(Directories.SharedLogs, "Logger_Settings.json");
		public static string GeneralSettingsFile => Path.Combine(Directories.LocalConfiguration, "Settings.json");
	}
}
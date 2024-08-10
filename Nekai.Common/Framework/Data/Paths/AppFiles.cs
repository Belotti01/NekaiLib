namespace Nekai.Common;

public static partial class NekaiData
{
	/// <summary>
	/// Contains the absolute paths to the files used by the framework.
	/// </summary>
	public static class Files
	{
		public static PathString LogSettingsFile => Directories.SharedLogs + "Logger_Settings.json";
		public static PathString GeneralSettingsFile => Directories.LocalConfiguration + "Settings.json";
	}
}
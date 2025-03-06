namespace Nekai.Razor;

public enum Theme
{
	None,
	Discovery,
	DiscoverySubtle,
	Primary,
	PrimarySubtle,
	Secondary,
	SecondarySubtle,
	Success,
	SuccessSubtle,
	Warning,
	WarningSubtle,
	Danger,
	DangerSubtle,
	Info,
	InfoSubtle,
	Light,
	LightSubtle,
	Dark,
	DarkSubtle
}

public static class ThemeExtensions
{
	private static string AsClassEnding(this Theme theme)
	{
		string name = theme.ToString();
		var parts = name.Split('S', 2);
		
		return parts.Length > 0 
			? $"{name.ToLower()}-subtle" 
			: name.ToLower();
	}

	public static string AsBackgroundColorClass(this Theme theme)
		=> theme == Theme.None ? "" : ("bg-" + theme.AsClassEnding());

	public static string AsBorderColorClass(this Theme theme)
		=> theme == Theme.None ? "" : ("border-" + theme.AsClassEnding());

	public static string AsTextColorClass(this Theme theme)
	// Can't be "text-color-subtle".
		=> theme == Theme.None ? "" : ("text-" + theme.ToString().Split('S', 1)[0]);

	public static string AsTextBackgroundClass(this Theme theme)
		=> theme == Theme.None ? "" : "text-bg-" + theme.ToString().Split('S', 1)[0];
}
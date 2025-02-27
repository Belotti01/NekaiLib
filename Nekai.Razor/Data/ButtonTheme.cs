namespace Nekai.Razor;

public enum ButtonTheme
{
	Default,
	Primary,
	Secondary,
	Success,
	Info,
	Light,
	Dark
}

public static class ButtonThemeExtensions
{
	public static string AsClass(this ButtonTheme theme)
		=> "btn-" + theme.ToString().ToLower();
}
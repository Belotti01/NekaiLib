namespace Nekai.Razor;

public enum Outline
{
	None,
	Primary,
	Secondary,
	Success,
	Danger,
	Warning,
	Info,
	Discovery,
	Light,
	Dark
}

public static class OutlineExtensions
{
	public static string AsClass(this Outline outline)
	{
		return outline == Outline.None
			? ""
			: "outline-" + outline.ToString().ToLower();
	}

	public static string AsButtonClass(this Outline outline)
	{
		var asClass = outline.AsClass();
		return asClass == ""
			? ""
			: "btn-" + asClass;
	}
}
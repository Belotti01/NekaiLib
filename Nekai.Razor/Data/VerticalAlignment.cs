namespace Nekai.Razor;

public enum VerticalAlignment
{
	Default,
	Top,
	Middle,
	Bottom
}

public static class VerticalAlignmentExtensions
{
	public static string ToCLass(this VerticalAlignment alignment)
		=> alignment switch
		{
			VerticalAlignment.Bottom => "align-bottom",
			VerticalAlignment.Top => "align-top",
			VerticalAlignment.Middle => "align-middle",
			_ => ""
		};
}
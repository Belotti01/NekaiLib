namespace Nekai.Razor;

public enum Position
{
	Default,
	Start,
	Top,
	End,
	Bottom
}

public static class PositionExtesions
{
	public static string ToFlexClass(this Position position)
	{
		return position switch
		{
			Position.Start => "d-inline-flex flex-row-reverse",
			Position.Top => "d-flex flex-column-reverse",
			Position.End => "d-inline-flex flex-row",
			Position.Bottom => "d-flex flex-column",
			_ => ""
		};
	}
}
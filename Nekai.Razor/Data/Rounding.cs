namespace Nekai.Razor;

public enum Rounding
{
	None,
	Rounded,
	Rounded2,
	Pill,
	Circle
}

public static class RoundingExtensions
{
	public static string AsClass(this Rounding rounding)
		=> rounding switch
		{
			Rounding.None => "rounded-0",
			Rounding.Rounded => "rounded-2",
			Rounding.Rounded2 => "rounded-3",
			_ => rounding.ToString().ToLower()
		};
}
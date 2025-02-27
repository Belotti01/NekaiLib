namespace Nekai.Razor;

public enum InputType
{
	Text,
	Password,
	Datetime,
	LocalDatetime,
	Date,
	Month,
	Time,
	Week,
	Number,
	Email,
	Url,
	Search,
	Tel,
	Color
}

public static class InputTypeExtensions
{
	public static string AsTypeString(this InputType type)
		=> type switch
		{
			InputType.LocalDatetime => "datetime-local",
			_ => type.ToString().ToLower()
		};
}
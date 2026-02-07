namespace Nekai.Common;

public static class BooleanExtensions
{

	public static string ToString(this bool value, BoolStringType type)
		=> type switch
		{
			BoolStringType.Default => value.ToString(),
			BoolStringType.TrueFalse => value ? "True" : "False",
			BoolStringType.YesNo => value ? "Yes" : "No",
			BoolStringType.OnOff => value ? "On" : "Off",
			BoolStringType.EnabledDisabled => value ? "Enabled" : "Disabled",
			_ => throw new NotImplementedException(),
		};
}
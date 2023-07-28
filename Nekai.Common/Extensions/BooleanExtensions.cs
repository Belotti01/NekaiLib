using System.Diagnostics;

namespace Nekai.Common;

public static class BooleanExtensions
{
#if DEBUG
	static BooleanExtensions()
	{
		_CheckBoolStringTypeForMissingImplementations();
	}

	[Conditional("DEBUG")]
	private static void _CheckBoolStringTypeForMissingImplementations()
	{
		BoolStringType[] types = Enum.GetValues<BoolStringType>();
		string s;

		foreach(var t in types)
        {
			try
			{
				s = true.ToString(t);
				s = false.ToString(t);
			}catch(NotImplementedException)
			{
				Debug.Fail($"Missing implementation of {nameof(BoolStringType)} \"{t}\" in extension method \"{nameof(BooleanExtensions)}.{nameof(ToString)}\".");
			}
        }
    }
#endif

	public static string ToString(this bool value, BoolStringType type)
		=> type switch
		{
			BoolStringType.Default => value.ToString(),
			BoolStringType.YesNo => value ? "Yes" : "No",
			BoolStringType.OnOff => value ? "On" : "Off",
			BoolStringType.EnabledDisabled => value ? "Enabled" : "Disabled",
			_ => throw new NotImplementedException(),
		};
}
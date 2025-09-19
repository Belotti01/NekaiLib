namespace Nekai.Common;

/// <summary>
/// Identifies the different <see cref="string"/> representations of a <see cref="bool"/> value.
/// </summary>
public enum BoolStringType
{
	/// <summary> "true" or "false" (default <see cref="bool.ToString()"/> behaviour).</summary>
	Default,
	
	/// <summary> "True" or "False" </summary>
	TrueFalse,

	/// <summary> "Yes" or "No".</summary>
	YesNo,

	/// <summary> "On" or "Off".</summary>
	OnOff,

	/// <summary> "Enabled" or "Disabled".</summary>
	EnabledDisabled
}
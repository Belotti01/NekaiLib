using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nekai.Common;

/// <summary>
/// Identifies the different <see cref="string"/> representations of a <see cref="bool"/> value.
/// </summary>
public enum BoolStringType
{
	/// <summary> "True" or "False" (default <see cref="bool.ToString()"/> behaviour).</summary>
	Default,
	/// <summary> "Yes" or "No".</summary>
	YesNo,
	/// <summary> "On" or "Off".</summary>
	OnOff,
	/// <summary> "Enabled" or "Disabled".</summary>
	EnabledDisabled
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nekai.Blazor.Components;

public enum PositioningType
{
	None,
	Start,
	End,
	Center,
	Between,
	Around,
	Baseline,
	Stretch
}

public static class JustifyTypeExtensions
{
	public static string? ToCss(this PositioningType positioning, string prefix)
	{
		return positioning switch
		{
			PositioningType.None => null,
			PositioningType.Start => $"{prefix}-start",
			PositioningType.End => $"{prefix}-end",
			PositioningType.Center => $"{prefix}-center",
			PositioningType.Between => $"{prefix}-between",
			PositioningType.Around => $"{prefix}-around",
			PositioningType.Stretch => $"{prefix}-stretch",
			PositioningType.Baseline => $"{prefix}-baseline",
			_ => Debugger.IsAttached
				? throw new ArgumentOutOfRangeException(nameof(positioning), positioning, null)
				: null
		};
	}
}

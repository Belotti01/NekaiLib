using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nekai.Blazor.Components;

public enum MarginType
{
	None = 0,
	Auto,
	Zero,
	Small,
	Medium,
	Large
}

public static class MarginTypeExtensions
{
	public static string? ToCssClass(this MarginType marginType, Dimension dimension)
	{
		return marginType switch
		{
			MarginType.None => null,
			MarginType.Auto => dimension.ToCss("m", "-auto"),
			MarginType.Zero => dimension.ToCss("m", "-0"),
			MarginType.Small => dimension.ToCss("m", "-1"),
			MarginType.Medium => dimension.ToCss("m", "-3"),
			MarginType.Large => dimension.ToCss("m", "-5"),
			_ => Debugger.IsAttached
					? throw new ArgumentOutOfRangeException(nameof(marginType), marginType, null)
					: null
		};
	}
}

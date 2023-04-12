using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nekai.Blazor.Components;

public enum PaddingType
{
	None = 0,
	Auto,
	Small,
	Medium,
	Large
}

public static class PaddingTypeExtensions
{
	public static string? ToCssClass(this PaddingType paddingType, Dimension dimension)
	{
		return paddingType switch
		{
			PaddingType.None => null,
			PaddingType.Auto => dimension.ToCss("p", "-auto"),
			PaddingType.Small => dimension.ToCss("p", "-1"),
			PaddingType.Medium => dimension.ToCss("p", "-3"),
			PaddingType.Large => dimension.ToCss("p", "-5"),
			_ => Debugger.IsAttached
					? throw new ArgumentOutOfRangeException(nameof(paddingType), paddingType, null)
					: null
		};
	}
}

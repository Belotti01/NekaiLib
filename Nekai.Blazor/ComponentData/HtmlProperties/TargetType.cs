using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nekai.Blazor.Components;

public enum TargetType
{
	Default,
	Self,
	Blank,
	Parent,
	Top
}

public static class TargetTypeExtensions
{
	public static string? ToHtmlValue(this TargetType targetType)
	{
		return targetType switch
		{
			TargetType.Default => null,
			TargetType.Self => "_self",
			TargetType.Blank => "_blank",
			TargetType.Parent => "_parent",
			TargetType.Top => "_top",
			_ => Debugger.IsAttached
				? throw new ArgumentOutOfRangeException(nameof(targetType), targetType, null)
				: null
		};
	}
}

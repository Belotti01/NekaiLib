using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nekai.Blazor.Components;

[Flags]
public enum Borders
{
	None = 0,
	Left = 1,
	Top = 2,
	Right = 4,
	Bottom = 8,
	Horizontal = Left | Right,
	Vertical = Top | Bottom,
	All = Horizontal | Vertical
}

public static class BorderTypeExtensions
{
	private static readonly IImmutableDictionary<Borders, string> _borderTypes = new Dictionary<Borders, string>()
	{
		{ Borders.Left, "border-left" },
		{ Borders.Top, "border-top" },
		{ Borders.Right, "border-right" },
		{ Borders.Bottom, "border-bottom" }
	}.ToImmutableDictionary();

	public static ReadOnlySpan<char> ToCssClass(this Borders borderType)
	{
		if(borderType == Borders.None)
			return default;

		if(borderType == Borders.All)
			return "border";

		return string.Join(' ', _borderTypes
			.Where(x => borderType.HasFlag(x.Key))
			.Select(x => x.Value));
	}
}

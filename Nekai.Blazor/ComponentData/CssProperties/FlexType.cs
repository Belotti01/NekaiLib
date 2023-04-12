using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nekai.Blazor.Components;

public enum FlexType
{
	None,
	Flex,
	Row,
	Column,
	RowReverse,
	ColumnReverse,
	Inline
}

public static class FlexTypeExtensions
{
	public static string? ToCssClass(this FlexType flexType)
	{
		return flexType switch
		{
			FlexType.None => null,
			FlexType.Flex => "d-flex",
			FlexType.Row => "d-flex flex-row",
			FlexType.Column => "d-flex flex-column",
			FlexType.RowReverse => "d-flex flex-row-reverse",
			FlexType.ColumnReverse => "d-flex flex-column-reverse",
			FlexType.Inline => "d-inline-flex",
			_ => Debugger.IsAttached
				? throw new ArgumentOutOfRangeException(nameof(flexType), flexType, null)
				: null
		};
	}

	public static DisplayType ToDisplayType(this FlexType flexType)
	{
		return flexType switch
		{
			FlexType.None => DisplayType.None,
			FlexType.Flex => DisplayType.Flex,
			FlexType.Row => DisplayType.FlexRow,
			FlexType.Column => DisplayType.FlexColumn,
			FlexType.RowReverse => DisplayType.FlexRowReverse,
			FlexType.ColumnReverse => DisplayType.FlexColumnReverse,
			FlexType.Inline => DisplayType.InlineFlex,
			_ => Debugger.IsAttached
				? throw new ArgumentOutOfRangeException(nameof(flexType), flexType, null)
				: DisplayType.None
		};
	}
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nekai.Blazor.Components;

public enum DisplayType
{
	None,
	Hidden,
	Inline,
	Flex,
	FlexRow,
	FlexRowReverse,
	FlexColumn,
	FlexColumnReverse,
	Block,
	InlineBlock,
	InlineFlex,
	Table,
	TableCell,
	TableRow
}

public static class DisplayTypeExtensions
{
	public static string? ToCssClass(this DisplayType displayType)
	{
		return displayType switch
		{
			DisplayType.None => null,
			DisplayType.Inline => "d-inline",
			DisplayType.Flex => FlexType.Flex.ToCssClass(),
			DisplayType.FlexRow => FlexType.Row.ToCssClass(),
			DisplayType.FlexRowReverse => FlexType.RowReverse.ToCssClass(),
			DisplayType.FlexColumn => FlexType.Column.ToCssClass(),
			DisplayType.FlexColumnReverse => FlexType.ColumnReverse.ToCssClass(),
			DisplayType.InlineFlex => FlexType.Inline.ToCssClass(),
			DisplayType.Block => "d-block",
			DisplayType.InlineBlock => "d-inline-block",
			DisplayType.Table => "d-table",
			DisplayType.TableRow => "d-table-row",
			DisplayType.TableCell => "d-table-cell",
			_ => Debugger.IsAttached
				? throw new ArgumentOutOfRangeException(nameof(displayType), displayType, null)
				: null
		};
	}
}

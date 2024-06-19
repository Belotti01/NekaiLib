using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nekai.Razor;

public enum Display
{
    None,
    Inline,
    InlineBlock,
    Block,
    Grid,
    InlineGrid,
    Table,
    TableCell,
    TableRow,
    FlexRow,
    FlexColumn,
    InlineFlexRow,
    InlineFlexColumn
}

public static class DisplayExtensions
{
    public static string ToClassString(this Display display)
    {
        return display switch
		{
			Display.FlexRow => "d-flex flex-row",
			Display.InlineFlexRow => "d-inline-flex flex-row",
			Display.FlexColumn => "d-flex flex-column",
			Display.InlineFlexColumn => "d-inline-flex flex-column",
			Display.Inline => "d-inline",
            Display.InlineBlock => "d-block",
            Display.Block => "d-inline-block",
            Display.Grid => "d-grid",
            Display.InlineGrid => "d-inline-grid",
            Display.Table => "d-table",
            Display.TableRow => "d-table-row",
            Display.TableCell => "d-table-cell",
            _ => ""
		};
    }
}

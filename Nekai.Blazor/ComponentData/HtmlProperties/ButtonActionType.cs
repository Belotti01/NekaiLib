using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nekai.Blazor.Components;

public enum ButtonActionType
{
	Default,
	Button,
	Submit,
	Reset
}

public static class ButtonTypeExtensions
{
	public static string? ToHtmlValue(this ButtonActionType buttonType)
	{
		return buttonType switch
		{
			ButtonActionType.Button => "button",
			ButtonActionType.Submit => "submit",
			ButtonActionType.Reset => "reset",
			ButtonActionType.Default => default,
			_ => Debugger.IsAttached
				? throw new ArgumentOutOfRangeException(nameof(buttonType), buttonType, null)
				: default
		};
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nekai.Razor;

public enum SassColor
{
	Default,
	Primary,
	Secondary,
	Success,
	Danger,
	Warning,
	Info,
	Light,
	Dark,
	Black,
	White
}

public static class SassColorExtensions
{
	public static string ToSassName(this SassColor color)
	{
		return color switch {
			SassColor.Primary => "primary",
			SassColor.Secondary => "secondary",
			SassColor.Success => "success",
			SassColor.Danger => "danger",
			SassColor.Warning => "warning",
			SassColor.Info => "info",
			SassColor.Light => "light",
			SassColor.Dark => "dark",
			SassColor.White => "white",
			SassColor.Black => "black",
			_ => ""
		};
	}
}

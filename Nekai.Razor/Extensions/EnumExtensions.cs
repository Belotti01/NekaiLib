using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nekai.Razor;

public static class EnumExtensions
{
	public static string ToBootStrapClass(this JustifyContent value)
	{
		return value switch {
			JustifyContent.SpaceBetween => "justify-content-between",
			JustifyContent.FlexStart => "justify-content-start",
			JustifyContent.FlexEnd => "justify-content-end",
			JustifyContent.SpaceAround => "justify-content-around",
			JustifyContent.Center => "justify-content-center",
			_ => ""
		};
	}

	public static string ToBootStrapAlignItemsClass(this Align value)
	{
		return value switch
		{
			Align.Start => "align-items-start",
			Align.End => "align-items-end",
			Align.Center => "align-items-center",
			Align.Stretch => "align-items-stretch",
			Align.Baseline => "align-items-baseline",
			_ => ""
		};
	}

	public static string ToBootStrapAlignSelfClass(this Align value)
	{
		return value switch
		{
			Align.Start => "align-self-start",
			Align.End => "align-self-end",
			Align.Center => "align-self-center",
			Align.Stretch => "align-self-stretch",
			Align.Baseline => "align-self-baseline",
			_ => ""
		};
	}
}

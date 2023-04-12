using System;

namespace Nekai.Blazor.Components;

public enum ScreenSize
{
	Default,
	XS,
	S,
	M,
	L,
	XL,
	XXL
}

public static class ScreenSizeExtensions 
{
	public static string ApplyToCssClass(this ScreenSize screenSize, string cssClass)
	{
		if(screenSize == ScreenSize.Default)
			return cssClass;
		
		string sizeFragment = screenSize switch
		{
			ScreenSize.XS => "xs",
			ScreenSize.S => "s",
			ScreenSize.M => "m",
			ScreenSize.L => "l",
			ScreenSize.XL => "xl",
			ScreenSize.XXL => "xxl",
			_ => throw new ArgumentOutOfRangeException(nameof(screenSize), screenSize, null),
		};

		int dashIndex = cssClass.IndexOf('-');
		if(dashIndex == -1)
			return cssClass + "-" + sizeFragment;
		
		if(dashIndex == 0)
			return sizeFragment + cssClass;

		return string.Concat(cssClass.AsSpan(0, dashIndex), "-", sizeFragment, cssClass.AsSpan(dashIndex));
	}
}

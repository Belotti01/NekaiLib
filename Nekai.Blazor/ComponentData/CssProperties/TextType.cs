using System.Diagnostics;

namespace Nekai.Blazor.Components;

public enum TextType
{
	P,
	H1,
	H2,
	H3,
	H4,
	H5,
	H6
}

public static class TextTypeExtensions
{
	public static ReadOnlySpan<char> ToHeaderTypeCssClass(this TextType headerType)
	{
		return headerType switch
		{
			TextType.H1 => "h1",
			TextType.H2 => "h2",
			TextType.H3 => "h3",
			TextType.H4 => "h4",
			TextType.H5 => "h5",
			TextType.H6 => "h6",
			TextType.P => default,
			_ => Debugger.IsAttached
				? throw new ArgumentOutOfRangeException(nameof(headerType), headerType, null)
				: default
		};
	}
	
	public static ReadOnlySpan<char> ToFontSizeCssClass(this TextType fontSize)
	{
		return fontSize switch
		{
			TextType.H1 => "fs-1",
			TextType.H2 => "fs-2",
			TextType.H3 => "fs-3",
			TextType.H4 => "fs-4",
			TextType.H5 => "fs-5",
			TextType.H6 => "fs-6",
			TextType.P => default,
			_ => Debugger.IsAttached
				? throw new ArgumentOutOfRangeException(nameof(fontSize), fontSize, null)
				: default
		};
	}
}

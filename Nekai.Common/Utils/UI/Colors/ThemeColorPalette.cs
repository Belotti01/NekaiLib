using System.Drawing;

namespace Nekai.Common;

public class ThemeColorPalette : ColorPalette
{
	[Configuration("Primary Main Color")]
	public Color Primary { get; set; }

	[Configuration("Primary Background Color")]
	public Color PrimaryBackground { get; set; }

	[Configuration("Primary Text Color")]
	public Color PrimaryText { get; set; }

	[Configuration("Secondary Main Color")]
	public Color Secondary { get; set; }

	[Configuration("Secondary Background Color")]
	public Color SecondaryBackground { get; set; }

	[Configuration("Secondary Text Color")]
	public Color SecondaryText { get; set; }

	[Configuration("Tertiary Main Color")]
	public Color Tertiary { get; set; }

	[Configuration("Tertiary Background Color")]
	public Color TertiaryBackground { get; set; }

	[Configuration("Tertiary Text Color")]
	public Color TertiaryText { get; set; }

	[Configuration("Highlighted Text Color")]
	public Color HighlightText { get; set; }

	[Configuration("Highlighted Background Color")]
	public Color HighlightBackground { get; set; }

	public ThemeColorPalette(string name)
		: base(NekaiData.Directories.Themes, name)
	{
	}
}
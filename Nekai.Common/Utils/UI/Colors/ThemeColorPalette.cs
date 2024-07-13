using System.Drawing;

namespace Nekai.Common;

public class ThemeColorPalette : JsonSerializableObject<ThemeColorPalette>
{
	[Configuration("Theme Name")]
	public string Name { get; }

    [Configuration("Primary Color")]
	public Color Primary { get; set; }

	[Configuration("Secondary Color")]
	public Color Secondary { get; set; }

	[Configuration("Info Color")]
	public Color Info { get; set; }

	[Configuration("Success Color")]
	public Color Success { get; set; }

	[Configuration("Danger Color")]
	public Color Danger { get; set; }

	[Configuration("White Color")]
	public Color White { get; set; } = Color.White;

	[Configuration("Black Color")]
	public Color Black { get; set; } = Color.Black;

    [Configuration("Light Color")]
    public Color Light { get; set; }

    [Configuration("Dark Color")]
    public Color Dark { get; set; }

    public ThemeColorPalette(string themeName)
		: base(NekaiData.Directories.Themes)
	{
		Name = themeName;
	}
}
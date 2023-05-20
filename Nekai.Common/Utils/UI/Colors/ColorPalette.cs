
namespace Nekai.Common;

public abstract class ColorPalette : ConfigurationFileManager<ColorPalette>
{
	public string Name { get; }

	public ColorPalette(string name)
		: base(Path.Combine(NekaiData.Directories.ColorPalettes, name + ".cfg"))
	{
		Name = name;
	}

	public ColorPalette(string folderPath, string name)
		: base(Path.Combine(folderPath, name + ".cfg"))
	{
		Name = name;
	}
}
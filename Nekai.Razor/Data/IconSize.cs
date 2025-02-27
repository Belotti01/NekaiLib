namespace Nekai.Razor;

/// <summary>
/// The Material Design Guidelines-compliant sizes for icons.
/// </summary>
/// <remarks>
/// Each enum's integer value indicates the applied size in pixels.
/// </remarks>
/// <seealso href="https://www.google.com/design/spec/iconography/system-icons.html"/>
public enum IconSize
{
	Small = 18,
	Default = 24,
	Big = 36,
	Maximum = 48
}

public static class IconSizeExtensions
{
	public static string ToPixelSize(this IconSize size)
		=> ((int)size) + "px";
}
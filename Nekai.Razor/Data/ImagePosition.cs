namespace Nekai.Razor;

public enum ImagePosition
{
	Default,
	Top,
	Bottom,
	Overlay,
	Fluid
}

public static class ImagePositionExtensions
{
	public static string AsCardImageClass(this ImagePosition position)
		=> position switch
		{
			ImagePosition.Top => "card-img-top",
			ImagePosition.Bottom => "card-img-bottom",
			ImagePosition.Overlay => "card-image-overlay",
			ImagePosition.Fluid => "img-fluid",
			_ => "card-img"
		};
}
namespace Nekai.Blazor.Components;

public interface INekaiContainer
{
	public ColorType BackgroundColor { get; set; }
	public ColorType BorderColor { get; set; }
	
	public int Padding { get; set; }
	public int XPadding { get; set; }
	public int YPadding { get; set; }

	public bool CenterContent { get; set; }
}

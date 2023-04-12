namespace Nekai.Blazor;

public interface INekaiStyle
{
	/// <summary>
	/// Classes that define this <see cref="INekaiStyle"/>.
	/// </summary>
	public string? Class { get; set; }

	/// <summary>
	/// CSS Style that defines this <see cref="INekaiStyle"/>.
	/// </summary>
	public string? Style { get; set; }
}

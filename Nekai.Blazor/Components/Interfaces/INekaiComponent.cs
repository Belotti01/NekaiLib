using Microsoft.AspNetCore.Components;

namespace Nekai.Blazor.Components;

public interface INekaiComponent : IComponent
{
	public string? Class { get; set; }
	public string? Style { get; set; }

	public Borders Borders { get; set; }
}

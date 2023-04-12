using Microsoft.AspNetCore.Components;

namespace Nekai.Blazor.Components;

public interface IDisableableComponent : IComponent
{
	public bool IsDisabled { get; set; }
}

using Microsoft.AspNetCore.Components;

namespace Nekai.Blazor.Components.Interfaces;

public interface IClickableComponent : IDisableableComponent
{
	public EventCallback OnClick { get; set; }
}

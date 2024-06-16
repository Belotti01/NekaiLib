using System;
using Microsoft.AspNetCore.Components;

namespace Nekai.Razor;

public class NekaiInteractiveBase : NekaiComponentBase
{
	[Parameter]
	public string? Name { get; set; }
	
}

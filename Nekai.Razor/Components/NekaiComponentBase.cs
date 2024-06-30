using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.FluentUI.AspNetCore.Components.DesignTokens;

namespace Nekai.Razor;

public class NekaiComponentBase : ComponentBase
{
    /// <summary>
    /// Classes to apply to the component.
    /// </summary>
    [Parameter]
	public string Class { get; set; } = "";

	/// <summary>
	/// Style to be applied to the component.
	/// </summary>
	[Parameter]
	public string Style { get; set; } = "";
}
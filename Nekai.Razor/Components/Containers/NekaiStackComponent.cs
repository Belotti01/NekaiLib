using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Nekai.Razor;

public class NekaiStackComponent : NekaiComponentBase
{
	[Parameter]
	public RenderFragment? ChildContent { get; set; }
	/// <summary> The CSS height value of the card. </summary>
	[Parameter]
	public string? Width { get; set; }
	/// <inheritdoc cref="FluentStack.HorizontalAlignment"/>
	[Parameter]
	public HorizontalAlignment HorizontalAlignment { get; set; }
	/// <inheritdoc cref="FluentStack.VerticalAlignment"/>
	[Parameter]
	public VerticalAlignment VerticalAlignment { get; set; }
	/// <inheritdoc cref="FluentStack.Wrap"/>
	[Parameter]
	public bool Wrap { get; set; }
	/// <inheritdoc cref="FluentStack.HorizontalGap"/>
	[Parameter]
	public int HorizontalGap { get; set; }
	/// <inheritdoc cref="FluentStack.VerticalGap"/>
	[Parameter]
	public int VerticalGap { get; set; }
}

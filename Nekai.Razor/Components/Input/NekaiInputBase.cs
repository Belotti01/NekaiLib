using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Nekai.Razor;

public class NekaiInputBase<T> : NekaiComponentBase
{
	protected override void OnParametersSet()
	{
		base.OnParametersSet();
		Name ??= Label;
	}

	/// <summary> The label shown for component. </summary>
	[Parameter]
	public string? Label { get; set; }

	/// <summary> The accessible name of this component. Defaults to the <see cref="Label"/> string. </summary>
	[Parameter]
	public string? Name { get; set; }

	/// <summary> The current input value. </summary>
	[Parameter]
	public T? Value { get; set; }

	[Parameter]
	public EventCallback<T> ValueChanged { get; set; }

	/// <summary> Whether the component is responsive or not. </summary>
	[Parameter]
	public bool Disabled { get; set; }

	/// <summary> Whether a value is required for this component. </summary>
	[Parameter]
	public bool Required { get; set; }

	/// <summary> The placeholder string to display when no value is set. </summary>
	[Parameter]
	public string? Placeholder { get; set; }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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

	/// <summary>
	/// The display mode to apply to the component.
	/// </summary>
	[Parameter]
	public Display Display { get; set; }

	/// <summary>
	/// Whether this component should resize itself depending on the screen size.
	/// </summary>
	[Parameter]
	public bool Responsive { get; set; }

	/// <summary>
	/// Whether this component should have a border or not.
	/// </summary>
	[Parameter]
	public bool Border { get; set; }

	/// <summary>
	/// Whether this component should have rounded edges.
	/// </summary>
	[Parameter]
	public bool Rounded { get; set; }

	/// <summary>
	/// The color of this component's border, if enabled.
	/// </summary>
	[Parameter]
	public SassColor BorderColor { get; set; }

	/// <summary>
	/// The color of this component's text.
	/// </summary>
	[Parameter]
	public SassColor TextColor { get; set; }

	/// <summary>
	/// The color of this component's background.
	/// </summary>
	[Parameter]
	public SassColor BackgroundColor { get; set; }

	/// <summary>
	/// The size of the shadow of this component; values from 0 (none) to 3 (large).
	/// </summary>
	[Parameter]
	public int Shadow { get; set; }

	protected override void OnParametersSet()
	{
		base.OnParametersSet();

		StringBuilder classBuilder = new(Class);
		OnClassBuilding(classBuilder);
		Class = classBuilder.ToString();
	}

	/// <summary>
	/// Called during the generation/extension of the <see cref="Class"/> for this component.
	/// </summary>
	/// <param name="classBuilder">The container of the generated <see cref="Class"/>.</param>
	protected virtual void OnClassBuilding(StringBuilder classBuilder)
	{
		if(Display != Display.None)
		{
			classBuilder.Append(' ').Append(Display.ToClassString());
		}

		if(Responsive)
		{
			classBuilder.Append(" container");
		}

		if(Shadow > 0)
		{
			classBuilder.Append(Shadow switch
			{
				1 => " shadow-sm",
				2 => " shadow",
				_ => " shadow-lg"
			});
		}

		if(Border)
		{
			classBuilder.Append(BorderColor == SassColor.Default
				? " border"
				: " border border-" + BorderColor.ToSassName());
		}

		if(Rounded)
		{
			classBuilder.Append(" rounded");
		}

		if(TextColor != SassColor.Default)
		{
			classBuilder.Append(" text-").Append(TextColor.ToSassName());
		}

		if(BackgroundColor != SassColor.Default)
		{
			classBuilder.Append(" bg-").Append(BackgroundColor.ToSassName());
		}
	}
}
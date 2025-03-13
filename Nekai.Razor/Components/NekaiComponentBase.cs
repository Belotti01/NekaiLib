using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web.HtmlRendering;

namespace Nekai.Razor;

public class NekaiComponentBase : ComponentBase
{
	[Inject]
	public SettingsManager Settings { get; set; } = null!;
	
	/// <summary>
	/// The ID of this component.
	/// </summary>
	[Parameter]
	public string Id { get; set; } = "";
	/// <summary>
	/// The classes to apply to this component.
	/// </summary>
	[Parameter]
	public string Class { get; set; } = "";
	/// <summary>
	/// The style to apply to this component.
	/// </summary>
	[Parameter]
	public string Style { get; set; } = "";
	/// <summary>
	/// The <c>aria-label</c> to use for this component.
	/// </summary>
	[Parameter]
	public string AriaLabel { get; set; } = "";
	/// <summary>
	/// The color of the text contained in this component.
	/// </summary>
	[Parameter]
	public Theme TextColor { get; set; }
	/// <summary>
	/// The color in the background of this component.
	/// </summary>
	[Parameter]
	public Theme BackgroundColor { get; set; }
	/// <summary>
	/// The color of the borders of this component.
	/// </summary>
	[Parameter]
	public Theme BorderColor { get; set; }

	protected override void OnParametersSet()
	{
		ApplyThemeColors();
		AfterClassSet();
		AfterStyleSet();
	}

	/// <summary> Called after setting the value of the <see cref="Class"/> property. </summary>
	public virtual void AfterClassSet() { }
	/// <summary> Called after setting the value of the <see cref="Style"/> property. </summary>
	public virtual void AfterStyleSet() { }

	/// <summary>
	/// Applies the theme colors as compiled by <see cref="GetThemeColorsClasses"/>.
	/// </summary>
	public virtual void ApplyThemeColors()
	{
		Class += ' ' + GetThemeColorsClasses();
	}

	/// <summary>
	/// Compile the color options into a list of classes.
	/// </summary>
	/// <returns> A <see langword="string"/> containing the list of classes. </returns>
	protected string GetThemeColorsClasses()
	{
		string classes = TextColor.AsTextColorClass();
		classes += ' ' + BorderColor.AsBorderColorClass();
		classes += ' ' + BackgroundColor.AsBackgroundColorClass();
		classes += ' ' + BackgroundColor.AsTextBackgroundClass();

		return classes.TrimEnd();
	}
}
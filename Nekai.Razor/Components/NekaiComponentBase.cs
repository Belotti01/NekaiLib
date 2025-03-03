using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web.HtmlRendering;

namespace Nekai.Razor;

public class NekaiComponentBase : ComponentBase
{
	[Inject]
	public SettingsManager Settings { get; set; } = null!;
	
	[Parameter]
	public string Id { get; set; } = "";
	[Parameter]
	public string Class { get; set; } = "";
	[Parameter]
	public string Style { get; set; } = "";
	[Parameter]
	public string AriaLabel { get; set; } = "";
	[Parameter]
	public Theme TextColor { get; set; }
	[Parameter]
	public Theme BackgroundColor { get; set; }
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

	public virtual void ApplyThemeColors()
	{
		Class += ' ' + GetThemeColorsClasses();
	}

	protected string GetThemeColorsClasses()
	{
		string classes = TextColor.AsTextColorClass();
		classes += ' ' + BorderColor.AsBorderColorClass();
		classes += ' ' + BackgroundColor.AsBackgroundColorClass();
		classes += ' ' + BackgroundColor.AsTextBackgroundClass();

		return classes.TrimEnd();
	}
}
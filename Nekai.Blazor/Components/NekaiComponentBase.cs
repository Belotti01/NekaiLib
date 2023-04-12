using Microsoft.AspNetCore.Components;
using Microsoft.Fast.Components.FluentUI;
using Nekai.Common;
using Serilog;

namespace Nekai.Blazor.Components;

public class NekaiComponentBase : FluentComponentBase, INekaiComponent
{
	protected Serilog.ILogger Logger => NekaiLogs.Program;

	[Parameter]
	public Borders Borders { get; set; }
	[Parameter]
	public int BorderSize { get; set; }
	[Parameter]
	public ColorType BorderColor { get; set; }
	[Parameter]
	public ColorType BackgroundColor { get; set; }
	[Parameter]
	public ColorType TextColor { get; set; }

	private ClassBuilder? _classBuilder;

	public override async Task SetParametersAsync(ParameterView parameters)
	{
		await base.SetParametersAsync(parameters);
		_BuildClass();
	}

	private void _BuildClass()
	{
		_classBuilder = new();
		OnStyling(_classBuilder);
		// The custom classes should be able to override any other value, so ensure that it is appended at the very end
		_classBuilder.WithClass(Class);
		Class = _classBuilder.Build();

		_classBuilder = null;
	}

	/// <summary>
	/// Invoked when the component is building the <see cref="FluentComponentBase.Class"/> and 
	/// <see cref="FluentComponentBase.Style"/> properties.
	/// </summary>
	/// <param name="classBuilder"> Object used to build or extend the <see cref="FluentComponentBase.Class"/> property of this 
	/// instance of the component. </param>
	/// <remarks>
	/// The <see cref="FluentComponentBase.Class"/> property is loaded by default and takes priority over any other value, 
	/// regardless of whether the base implementation of this method is invoked or not.
	/// </remarks>
	protected virtual void OnStyling(ClassBuilder classBuilder)
	{
		classBuilder
			.WithTextColor(TextColor)
			.WithBackgroundColor(BackgroundColor)
			.WithBorders(Borders, BorderSize, BorderColor);
	}
}

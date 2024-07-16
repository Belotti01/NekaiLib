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
	public bool IsExecutingBackgroundTask { get; private set; } = false;
	private Task? _backgroundTask;
	protected CancellationTokenSource BackgroundTaskCancellationTokenSource { get; } = new();
	protected CancellationToken BackgroundTaskCancellationToken { get; private set; }

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

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();

		BackgroundTaskCancellationToken = BackgroundTaskCancellationTokenSource.Token;
		IsExecutingBackgroundTask = true;
		_backgroundTask = Task
			.Run(async () => await ExecuteInBackground(BackgroundTaskCancellationToken), BackgroundTaskCancellationToken)
			.ContinueWith((t) => IsExecutingBackgroundTask = false);
	}

	/// <summary>
	/// Method executed in the background, initiated upon the call to <see cref="OnInitializedAsync"/>.
	/// </summary>
	/// <param name="cancellationToken"></param>
	/// <returns></returns>
	protected virtual async Task ExecuteInBackground(CancellationToken cancellationToken)
	{

	}
}
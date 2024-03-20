using System.Diagnostics;

namespace Nekai.Common;

/// <summary>
/// Contains information regarding the executing application and the host it's running on.
/// </summary>
public static class NekaiApp
{
	/// <summary> Whether the application is running in DEBUG mode. </summary>
	public const bool IS_DEBUG
#if DEBUG
		= true;
#else
		= false;
#endif
	/// <summary> Whether the application is running in DEBUG mode and has a debugger attached. </summary>
	public static bool IsDebugWithDebugger
		=> IS_DEBUG && HasDebugger;
	/// <summary> Whether the application is running with a debugger attached. </summary>
	public static bool HasDebugger
		=> Debugger.IsAttached;

	/// <inheritdoc cref="AppDomain.FriendlyName"/>
	public static string Name => AppDomain.CurrentDomain.FriendlyName;

	/// <summary>
	/// Network information of the host on which this application is running.
	/// </summary>
	public static ProcessHostInformationLoader LocalHost { get; } = new(false);

	/// <inheritdoc cref="AppDomain.ProcessExit"/>
	public static event EventHandler? OnProcessExit;
	/// <summary>
	/// Invoked when the Application is about to exit, and all the handlers attached to <see cref="OnProcessExit"/>
	/// have completed execution.
	/// </summary>
	/// <remarks>
	/// Internal event, used to free the resources used by the framework that might be required by previous handlers.
	/// Avoid any dependence to objects outside of the scope of the handler, since they might already have been disposed.
	/// </remarks>
	internal static event EventHandler? _OnProcessExitHandledInternal;



	static NekaiApp()
	{
		AppDomain.CurrentDomain.ProcessExit += (sender, args) => OnProcessExit?.Invoke(sender, args);
	}



	private static void _ProcessExitHandler(object? sender, EventArgs e)
	{
		try
		{
			OnProcessExit?.Invoke(sender, e);
		}
		catch(Exception ex)
		{
			NekaiLogs.Program.Error($"An unhandled Exception was caught while closing the application: {ex.Message}");
		}
		OnProcessExit = null;

		try
		{
			_OnProcessExitHandledInternal?.Invoke(sender, e);
		}
		catch(Exception ex)
		{
			Exceptor.ThrowIfDebug($"An unhandled Exception was caught while closing the application: {ex.Message}", ex);
		}
		_OnProcessExitHandledInternal = null;
	}
}
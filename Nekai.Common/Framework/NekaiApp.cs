using System.Configuration;
using System.Diagnostics;
using System.Net;

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
	/// Load the current host's network information.
	/// </summary>
	public static IPHostEntry LocalHost => Dns.GetHostEntry(LocalHostName);

	/// <summary>
	/// The network hostname.
	/// </summary>
	public static string LocalHostName => Dns.GetHostName();

	/// <inheritdoc cref="AppDomain.ProcessExit"/>
	public static event EventHandler? OnProcessExit;

	/// <summary>
	/// Invoked when the Application is about to exit, and all the handlers attached to <see cref="OnProcessExit"/>
	/// have completed execution.
	/// </summary>
	/// <remarks>
	/// Internal event, used to free the resources used by the framework that might be required by previous handlers.
	/// Avoid any dependence to objects outside the scope of the handler, since they might already have been disposed.
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
	
	public static Configuration ReadConfiguration(ConfigurationUserLevel level = ConfigurationUserLevel.PerUserRoamingAndLocal)
	{
		return ConfigurationManager
			.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal);
	}

	/// <inheritdoc cref="GCMemoryInfo.TotalCommittedBytes"/>
	public static long GetUsedHeapMemory()
	{
		return GC.GetGCMemoryInfo().TotalCommittedBytes;
	}

	/// <inheritdoc cref="GC.GetTotalMemory(bool)"/>
	public static long GetFreeHeapMemory()
	{
		return GC.GetTotalMemory(false);
	}

	/// <inheritdoc cref="GCMemoryInfo.HeapSizeBytes"/>
	public static long GetTotalHeapMemory()
	{
		return GC.GetGCMemoryInfo().HeapSizeBytes;
	}

	/// <summary>
	/// Check whether the memory usage of the last GC invocation is above the "High-load" threshold.
	/// </summary>
	public static bool IsMemoryUnderPressure()
	{
		var memoryInfo = GC.GetGCMemoryInfo();
		return memoryInfo.MemoryLoadBytes > memoryInfo.HighMemoryLoadThresholdBytes;
	}
}
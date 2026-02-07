using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using Microsoft.Extensions.Configuration;
using ConfigurationManager=Microsoft.Extensions.Configuration.ConfigurationManager;

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

	/// <summary> The general configuration. </summary>
	/// <remarks> See: <see cref="NekaiGeneralConfiguration.Singleton"/></remarks>
	public static NekaiGeneralConfiguration Configuration => NekaiGeneralConfiguration.Singleton;
	
	/// <summary> Whether the application is running in DEBUG mode and has a debugger attached. </summary>
	public static bool IsDebugWithDebugger
		=> IS_DEBUG && HasDebugger;

	/// <summary> Whether the application is running with a debugger attached. </summary>
	public static bool HasDebugger
		=> Debugger.IsAttached;

	/// <inheritdoc cref="AppDomain.FriendlyName"/>
	public static string Name => AppDomain.CurrentDomain.FriendlyName;

	/// <summary> Load the current host's network information. </summary>
	public static IPHostEntry LocalHost => Dns.GetHostEntry(LocalHostName);

	/// <summary> The configuration stored in appsettings.json. </summary>
	public static IConfiguration AppSettings
	{
		get
		{
			if(field is not null)
				return field;
			
			var result = PathString.TryParse("appsettings.json");
			if(result.IsSuccessful && TryParseConfigurationFile(result.Value, out IConfiguration? configuration))
			{
				field = configuration;
				return configuration;
			}
			// Parsing failed.
			NekaiLogs.Program.Warning("Attempted to read AppSettings, but no appsettings.json file was found.");
			return new ConfigurationManager();
		}
	}
	
	/// <summary> The network hostname. </summary>
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
	internal static event EventHandler? OnProcessExitHandledInternal;

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
			NekaiLogs.Program.Error("An unhandled Exception was caught while closing the application: {exception}", ex.Message);
		}
		OnProcessExit = null;

		try
		{
			OnProcessExitHandledInternal?.Invoke(sender, e);
		}
		catch(Exception ex)
		{
			Exceptor.ThrowIfDebug($"An unhandled Exception was caught while closing the application: {ex.Message}", ex);
		}
		OnProcessExitHandledInternal = null;
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

	/// <summary>
	/// Attempt to parse a JSON configuration file, and return the resulting <see cref="IConfiguration"/>.
	/// </summary>
	/// <param name="path">The path of the configuration file to parse.</param>
	/// <param name="configuration">The resulting <see cref="IConfiguration"/>, or <see langword="false"/> if the file at <paramref name="path"/> doesn't exist.</param>
	/// <returns><see langword="true"/> if the <paramref name="configuration"/> has been built, or <see langword="false"/>
	/// if the file at <paramref name="path"/> doesn't exist.</returns>
	public static bool TryParseConfigurationFile(PathString path, [NotNullWhen(true)] out IConfiguration? configuration)
	{
		if(!path.IsExistingFile())
		{
			configuration = null;
			return false;
		}
			
		configuration = new ConfigurationBuilder()
			.AddJsonFile(path)
			.Build();

		return true;
	}

	/// <summary>
	/// Attempt to start the system's default browser at the <paramref name="url"/> page.
	/// </summary>
	/// <param name="url">The page to open in the browser.</param>
	/// <param name="process">The started <see cref="Process"/>, or <see langword="null"/> if the operation failed.</param>
	/// <returns><see langword="true"/> if the browser was opened correctly; <see langword="false"/> otherwise.</returns>
	public static bool TryLaunchInBrowser(string url, [NotNullWhen(true)] out Process? process)
	{
		try
		{
			process = Process.Start(new ProcessStartInfo(url)
			{
				UseShellExecute = true,
			});
			return process is not null;
		}
		catch(Exception ex)
		{
			NekaiLogs.Program.Error("Could not start browser process at url \"{url}\": {exception}.", url, ex.Message);
			process = null;
			return false;
		}
	}
}
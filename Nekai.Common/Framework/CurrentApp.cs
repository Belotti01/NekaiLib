using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace Nekai.Common;

/// <summary>
/// Contains information regarding the executing application and the host it's running on.
/// </summary>
public static class CurrentApp
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

	private static IPAddress? _hostIp;
	/// <summary> InterNetwork IP Address of the host on which this application is running. </summary>
	/// <remarks> If the DNS failed to retrieve the required information, this identifies the local IP Address instead (127.0.0.1). </remarks>
	public static IPAddress HostIp
	{ 
		get
		{
			if(_hostIp is not null)
				return _hostIp;

			var result = _TryRetrieveLocalHostIpAddress();
			if(result.IsSuccess)
			{
				_hostIp = result.Value;
				return _hostIp;
			}

			NekaiLogs.Program.Error(result.Message);
			NekaiLogs.Program.Error("Could not retrieve Inter-Network IP Address for localhost. Using 127.0.0.1 as fallback.");
			_hostIp = new(new byte[] { 127, 0, 0, 1 });
			return _hostIp;
		}
	}
	private static string? _localHostName;
	/// <summary> HostName of the host on which this application is running, as identified by the DNS. </summary>
	/// <remarks> If the DNS failed to retrieve the required information, this will return "localhost" instead. </remarks>
	public static string LocalHostName
	{ 
		get
		{
			if(_localHostName is not null)
				return _localHostName;

			var result = _TryGetLocalHostName();
			if(result.IsSuccess)
			{
				_localHostName = result.Value;
				return _localHostName;
			}

			NekaiLogs.Program.Error(result.Message);
			NekaiLogs.Program.Error("Failed to retrieve local Hostname. Using \"localhost\" as fallback.");
			_localHostName = "localhost";
			return _localHostName;
		} 
	}

	/// <inheritdoc cref="AppDomain.ProcessExit"/>
	public static event EventHandler? OnProcessExit;
	/// <summary>
	/// Invoked when the Application is about to exit, and all the handlers attached to <see cref="OnProcessExit"/>
	/// have completed execution.
	/// </summary>
	/// <remarks>
	/// Internal event, used to free the resources used by the framework that might be required by previous handlers.
	/// Avoid logging in this event, as the <see cref="NekaiLogs"/> might have already been disposed.
	/// </remarks>
	internal static event EventHandler? _OnProcessExitHandledInternal;

	static CurrentApp()
	{
		AppDomain.CurrentDomain.ProcessExit += (sender, args) => OnProcessExit?.Invoke(sender, args);
	}

	private static void _ProcessExitHandler(object? sender, EventArgs e)
	{
		try
		{
			OnProcessExit?.Invoke(sender, e);
		}catch(Exception ex)
		{
			NekaiLogs.Program.Error($"An unhandled Exception was caught while closing the application: {ex.Message}");
		}

		try
		{
			_OnProcessExitHandledInternal?.Invoke(sender, e);
		}catch(Exception ex)
		{
			NekaiLogs.Program.Error($"An unhandled Exception was caught while closing the application: {ex.Message}");
		}
	}

	private static Result<string> _TryGetLocalHostName()
	{
		try
		{
			string localHostname = Dns.GetHostName();
			return Result<string>.Success(localHostname);
		}catch(SocketException ex)
		{
			return Result<string>.Failure("Could not retrieve localhost's hostname: " + ex.Message);
		}
	}

	private static Result<IPAddress> _TryRetrieveLocalHostIpAddress()
	{
		// Tip: most DNS issues can be fixed by:
		// - Using 8.8.8.8 (or 8.8.4.4), or just trying other DNSs
		// - Flushing the local DNS cache
		IPAddress? localHostIp;

		try
		{
			// Query the DNS and extract the Inter-Network IP address (so avoid 127.0.0.1)
			localHostIp = Dns.GetHostEntry(LocalHostName)
				.AddressList
				.FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork);

			if(localHostIp is null)
				// The DNS responded, but no InterNetwork IP was found. Might be a caching issue.
				return Result<IPAddress>.Failure("Localhost Inter-Network IP Address not found.");
			return Result.Success(localHostIp);
		}catch(Exception ex)
		{
			return Result<IPAddress>.Failure("Localhost Inter-Network IP Address could not be loaded: " + ex.Message);
		}
	}
}
using System.Collections.Immutable;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;

namespace Nekai.Common;

/// <summary>
/// Object that can be used to query the DNS for network information about the host on which the application is running.
/// </summary>
public class ProcessHostInformationLoader
{
	/// <summary> The fallback <see cref="HostName"/> value used when an Inter-Network hostname cannot be retrieved. </summary>
	public const string DEFAULT_HOSTNAME = "localhost";
	/// <summary> The fallback <see cref="IPAddress"/> value used when an Inter-Network ip address cannot be retrieved. </summary>
	public static IPAddress DefaultIpAddress { get; } = new(new byte[] { 127, 0, 0, 1 });
	/// <summary> Array of all the IP addresses associated with the current process' host. </summary>
	public ImmutableArray<IPAddress> IpAddresses { get; protected set; }
	/// <summary> The result of the last attempt to load the host information. </summary>
	public NetworkOperationResult LastLoadingOperationResult { get; protected set; }

	/// <summary>
	/// Whether the <see cref="IPAddress"/> and <see cref="HostName"/> properties have been successfully set to
	/// this host's Inter-Network IP Address and Host Name. If <see langword="false"/>, the properties are
	/// set to the local identifiers as fallback.
	/// </summary>
	public bool HasInterNetworkInfo => !(_hostName == DEFAULT_HOSTNAME || ReferenceEquals(_ipAddress, DefaultIpAddress));

	private IPAddress? _ipAddress;
	/// <summary> Inter-Network IP Address of the host on which this application is running. </summary>
	/// <remarks> If the DNS failed to retrieve the required information, this will return <see cref="DefaultIpAddress"/> instead. </remarks>
	public IPAddress IPAddress
	{
		get
		{
			if(_ipAddress is not null)
				return _ipAddress;

			_ipAddress = _TryRetrieveLocalHostIpAddress().Value ?? DefaultIpAddress;
			return _ipAddress;
		}
	}

	/// <summary> Name of the host on which this application is running, as identified by the DNS. </summary>
	/// <remarks> If the DNS failed to retrieve the required information, this will return <see cref="DEFAULT_HOSTNAME"/> instead. </remarks>
	public string HostName
	{
		get
		{
			if(_hostName is not null)
				return _hostName;

			_hostName = _TryGetLocalHostName().Value ?? DEFAULT_HOSTNAME;
			return _hostName;
		}
	}
	private static string? _hostName;

	public ProcessHostInformationLoader(bool preloadInfo = true)
	{
		if(preloadInfo)
		{
			var result = TryUpdate();
			Debug.Assert(result is NetworkOperationResult.Success or NetworkOperationResult.NoInternet, "Failed to query DNS for inter-network localhost information.");
		}
	}

	public NetworkOperationResult TryUpdate()
	{
		// The HostName is used by the IP address retrieval method, so load it first.
		var nameResult = _TryGetLocalHostName();
		if(!nameResult.IsSuccessful)
		{
			// Don't overwrite previously loaded value.
			_hostName ??= DEFAULT_HOSTNAME;
			_ipAddress ??= DefaultIpAddress;
			LastLoadingOperationResult = nameResult.Error;
			return LastLoadingOperationResult;
		}
		_hostName = nameResult.Value;

		var ipResult = _TryRetrieveLocalHostIpAddress();
		if(!ipResult.IsSuccessful)
		{
			_ipAddress ??= DefaultIpAddress;
			LastLoadingOperationResult = ipResult.Error;
			return LastLoadingOperationResult;
		}

		_ipAddress = ipResult.Value;
		LastLoadingOperationResult = NetworkOperationResult.Success;
		return LastLoadingOperationResult;
	}

	private static Result<string, NetworkOperationResult> _TryGetLocalHostName()
	{
		try
		{
			string localHostname = Dns.GetHostName();
			return localHostname;
		}
		catch(SocketException ex)
		{
			NekaiLogs.Shared.Error($"DNS request failed: {ex.Message}.");
			return new(NetworkOperationResult.DnsError);
		}
	}

	private Result<IPAddress, NetworkOperationResult> _TryRetrieveLocalHostIpAddress()
	{
		// Random tip: most DNS issues can be fixed by:
		// - Setting a public DNS in the machine's network settings (f.e. Google's 8.8.8.8/8.8.4.4 or Cloudflare's 1.1.1.1)
		// - Flushing the machine's DNS cache
		IPAddress? localHostIp;

		try
		{
			// Query the DNS and extract the Inter-Network IP address (so avoid 127.0.0.1)
			IpAddresses = Dns.GetHostEntry(HostName)
				.AddressList
				.ToImmutableArray();

			localHostIp = IpAddresses
				.Where(x => x.AddressFamily == AddressFamily.InterNetwork)
				.FirstOrDefault();

			if(localHostIp is null)
				// The DNS responded, but no Inter-Network IP was found.
				return new(NetworkOperationResult.DnsError);
			return localHostIp;
		}
		catch(SocketException ex)
		{
			NekaiLogs.Shared.Error($"DNS request failed: {ex.Message}.");
			return new(NetworkOperationResult.DnsError);
		}
		catch(Exception ex)
		{
			Debug.Fail("Check arguments for null or invalid values.");
			NekaiLogs.Shared.Error($"DNS request failed: {ex.Message}.");
			return new(NetworkOperationResult.UnknownError);
		}
	}

	/// <summary>
	/// Return the string representation of this <see cref="ProcessHostInformationLoader"/>.
	/// </summary>
	/// <returns> A <see langword="string"/> containing the values of <see cref="HostName"/> and <see cref="IPAddress"/>  in IPv4 format. </returns>
	public override string ToString()
		=> ToIPv4String();	// Just use the conventional format (IPv4) for the default string representation.


	/// <summary>
	/// Return the string representation of this <see cref="ProcessHostInformationLoader"/>, using IPv6 addresses.
	/// </summary>
	/// <returns> A <see langword="string"/> containing the values of <see cref="HostName"/> and <see cref="IPAddress"/> in IPv6 format. </returns>
	public string ToIPv6String()
	{
		var ipv6Addresses = IpAddresses
			.Select(x => x.MapToIPv6());
		return _ToStringInternal(ipv6Addresses, HostName);
	}

	/// <summary>
	/// Return the string representation of this <see cref="ProcessHostInformationLoader"/>, using IPv4 addresses.
	/// </summary>
	/// <returns> A <see langword="string"/> containing the values of <see cref="HostName"/> and <see cref="IPAddress"/>. </returns>
	public string ToIPv4String()
	{
		var ipv4Addresses = IpAddresses
			.Select(x => x.MapToIPv4());
		return _ToStringInternal(ipv4Addresses, HostName);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static string _ToStringInternal(IEnumerable<IPAddress> addresses, string hostName)
	{
		if(addresses is null)
			return $"[{hostName}]";

		string list = string.Join("; ", addresses);
		return $"[{hostName}] {list}";
	}
}

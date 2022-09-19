using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Nekai.Common;

public class NetEndPoint {

	public string Hostname => DnsInfo is null
		? IPv4Address.ToString()
		: DnsInfo.Host;
	public string IPv4 => IPv4Address.ToString();
	public string IPv6 => IPv6Address.ToString();

	public IPAddress IPv4Address { get; protected set; }
	public IPAddress IPv6Address { get; protected set; }

	private Lazy<IPHostEntry?> _IPHostInfoLoader { get; init; }
	public IPHostEntry? IPHostInfo => _IPHostInfoLoader.Value;
	private Lazy<IPEndPoint?> _IPEndPointInfoLoader { get; init; }
	public IPEndPoint? IPEndPointInfo => _IPEndPointInfoLoader.Value;
	private Lazy<DnsEndPoint?> _DnsInfoLoader { get; init; }
	public DnsEndPoint? DnsInfo => _DnsInfoLoader.Value;
	public int? Port { get; protected set; }
	

	public NetEndPoint(string ipOrHostname, int port)
		: this(Dns.GetHostEntry(ipOrHostname).AddressList[0], port)
	{
	}

	public NetEndPoint(IPAddress address, int port)
	{
		IPv4Address = address.MapToIPv4();
		IPv6Address = address.MapToIPv6();
		if(port is >= IPEndPoint.MinPort and <= IPEndPoint.MaxPort)
		{
			Port = port;
		}

		_IPHostInfoLoader = new(LoadIPHostInfo);
		_DnsInfoLoader = new(LoadDnsInfo);
		_IPEndPointInfoLoader = new(LoadIPEndPointInfo);
	}

	public static bool TryParse(string ipOrHostname, int port, [NotNullWhen(true)] out NetEndPoint? endPoint)
	{
		endPoint = null;
		try
		{
			endPoint = new(ipOrHostname, port);
		}catch { }
		return endPoint is not null;
	}

	public static bool TryParse(IPAddress address, int port, [NotNullWhen(true)] out NetEndPoint? endPoint)
	{
		endPoint = null;
		try
		{
			endPoint = new(address, port);
		} catch { }
		return endPoint is not null;
	}



	protected virtual IPHostEntry? LoadIPHostInfo()
	{
		try
		{
			return Dns.GetHostEntry(IPv4Address);
		} catch
		{
			return null;
		}
	}

	protected virtual DnsEndPoint? LoadDnsInfo()
	{
		if(Port.HasValue)
		{
			try
			{
				return new(Hostname, Port.Value);
			} catch { }
		}
		return null;
	}

	protected virtual IPEndPoint? LoadIPEndPointInfo()
	{
		if(Port.HasValue)
		{
			try
			{
				return new(IPv4Address, Port.Value);
			} catch { }
		}
		return null;
	}


	public override string ToString()
	{
		return Port.HasValue
			? $"{Hostname}:{Port}"
			: Hostname;
	}
}

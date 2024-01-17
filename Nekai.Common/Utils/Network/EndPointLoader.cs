using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Sockets;

namespace Nekai.Common;

public class EndPointLoader
{
	public int Port { get; protected set; }
	public IPAddress Address { get; private set; }
	private IPEndPoint? _endPoint;
	public IPEndPoint EndPoint => _endPoint ??= new(Address, Port);
	private IPHostEntry? _hostEntry;
	public IPHostEntry HostEntry => _hostEntry ??= Dns.GetHostEntry(Address);

	protected EndPointLoader(string hostnameOrIp, AddressFamily addressFamily = AddressFamily.Unspecified)
	{
		ReadOnlySpan<char> ipSpan = hostnameOrIp.AsSpan();
		if(TryExtractPort(ipSpan, out string hostnameOrIpWithoutPort, out int port))
		{
			Port = port;
			ThrowIfInvalidPort(nameof(hostnameOrIp));
			LoadIpAddress(hostnameOrIpWithoutPort, addressFamily);
		}
		else
		{
			Port = 0;
			LoadIpAddress(hostnameOrIp);
		}
	}

	protected EndPointLoader(string hostnameOrIp, int port, AddressFamily addressFamily = AddressFamily.Unspecified)
	{
		Port = port;
		ThrowIfInvalidPort(nameof(hostnameOrIp));
		LoadIpAddress(hostnameOrIp, addressFamily);
	}

	public static EndPointLoader Parse(string hostnameOrIp, AddressFamily addressFamily = AddressFamily.Unspecified)
		=> new(hostnameOrIp);

	public static EndPointLoader Parse(string hostnameOrIp, int port, AddressFamily addressFamily = AddressFamily.Unspecified)
		=> new(hostnameOrIp, port);

	public static bool TryParse(string hostnameOrIp, [NotNullWhen(true)] out EndPointLoader? endPointLoader)
		=> TryParse(hostnameOrIp, AddressFamily.Unspecified, out endPointLoader);

	public static bool TryParse(string hostnameOrIp, AddressFamily addressFamily, [NotNullWhen(true)] out EndPointLoader? endPointLoader)
	{
		try
		{
			endPointLoader = new(hostnameOrIp);
			return true;
		}
		catch
		{
			endPointLoader = null;
			return false;
		}
	}

	public static bool TryParse(string hostnameOrIp, int port, [NotNullWhen(true)] out EndPointLoader? endPointLoader)
		=> TryParse(hostnameOrIp, port, AddressFamily.Unspecified, out endPointLoader);

	public static bool TryParse(string hostnameOrIp, int port, AddressFamily addressFamily, [NotNullWhen(true)] out EndPointLoader? endPointLoader)
	{
		try
		{
			endPointLoader = new(hostnameOrIp, port, addressFamily);
			return true;
		}
		catch
		{
			endPointLoader = null;
			return false;
		}
	}

	protected static bool TryExtractPort(ReadOnlySpan<char> ipOrHostnameView, out string ipOrHostnameWithoutPort, out int port)
	{
		int maxPortLength = IPEndPoint.MaxPort.CountDigits();

		for(int i = ipOrHostnameView.Length - 1; i >= 0; --i)
		{
			if(ipOrHostnameView[i] == ':')
			{
				ReadOnlySpan<char> portView = ipOrHostnameView[(i + 1)..];
				port = int.Parse(portView);
				ipOrHostnameWithoutPort = i <= 1
					? ""
					: ipOrHostnameView[..(i - 1)].ToString();
				return true;
			}

			// Port is at most 5 digits long, and cannot contanon-numeric characters
			if(!ipOrHostnameView[i].IsNumber() || i < ipOrHostnameView.Length - maxPortLength)
				break;
		}

		ipOrHostnameWithoutPort = ipOrHostnameView.ToString();
		port = 0;
		return false;
	}

	[MemberNotNull(nameof(Address))]
	protected void LoadIpAddress(string hostnameOrIp, AddressFamily addressFamily = AddressFamily.Unspecified)
	{
		var addresses = Dns.GetHostAddresses(hostnameOrIp, addressFamily);
		if(!addresses.Any())
		{
			throw new ArgumentException("Hostname or IP address does not exist.", nameof(hostnameOrIp));
		}
		Address = addresses[0];
	}

	protected void ThrowIfInvalidPort([ConstantExpected] string paramName)
	{
		if(Port is < IPEndPoint.MinPort or > IPEndPoint.MaxPort)
			throw new ArgumentOutOfRangeException(paramName, $"Port must be between {IPEndPoint.MinPort} and {IPEndPoint.MaxPort}.");
	}
}
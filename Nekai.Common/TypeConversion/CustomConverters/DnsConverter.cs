using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Nekai.Common;

public class DnsConverter : ICustomConverter {
	public Type Type => typeof(DnsEndPoint);

	public object? ConvertFromString(string value)
	{
		string[] parts = value.Split(':');
		if(parts.Length < 2 || !int.TryParse(parts[^1], out int port))
		{
			IPHostEntry hostEntry;
			try
			{
				hostEntry = Dns.GetHostEntry(value);
			}catch
			{
				return null;
			}

			if(hostEntry.AddressList.Length == 0)
				return null;
			
			var ip = hostEntry.AddressList[0];
			return new DnsEndPoint(hostEntry.HostName, 80, ip.AddressFamily);
		}

		try
		{
			string ip = string.Join(':', parts[..^1]);
			return new DnsEndPoint(ip, port);
		} catch
		{
			return null;
		}
	}

	public string ConvertToString(object value)
	{
		return (value as DnsEndPoint)?.ToString() ?? "";
	}
}

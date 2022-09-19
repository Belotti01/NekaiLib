using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Nekai.Common;

public class IpAddressConverter : ICustomConverter {
	public Type Type => typeof(IPAddress);

	public object? ConvertFromString(string value)
	{
		if(IPAddress.TryParse(value, out var ip))
			return ip;
		return null;
	}

	public string ConvertToString(object value)
	{
		return (value as IPAddress)?.ToString() ?? IPAddress.None.ToString();
	}
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Nekai.Common;

public struct NekaiProcessNetworkInfo
{
	public string Name { get; }
	public string Host { get; }
	public int Port { get; }
	public int ProcessId { get; }
	public ProtocolType Protocol { get; }

	public NekaiProcessNetworkInfo(string name, string host, int port, int processId, ProtocolType protocol)
	{
		Name = name;
		Host = host;
		Port = port;
		ProcessId = processId;
		Protocol = protocol;
	}

	public static NekaiProcessNetworkInfo ForCurrentProcess(ProtocolType protocol, int port)
	{
		string hostIp = CurrentApp.HostIp.MapToIPv4().ToString();
		return new(CurrentApp.Name, hostIp, port, Environment.ProcessId, protocol);
	}
}

using System.Net.Sockets;

namespace Nekai.Common;

public sealed class NekaiProcessNetworkInfo
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
		string hostIp = NekaiApp.LocalHost.IPAddress.MapToIPv4().ToString();
		return new(NekaiApp.Name, hostIp, port, Environment.ProcessId, protocol);
	}
}

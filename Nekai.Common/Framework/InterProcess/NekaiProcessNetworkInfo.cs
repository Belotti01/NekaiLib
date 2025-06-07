using System.Diagnostics;
using System.Net;
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
		IPAddress? address = NekaiApp.LocalHost.AddressList.FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork);
		if(address is null)
		{
			NekaiLogs.Shared.Warning("Couldn't load InterNetwork-type IP for the process' host.");
			Debug.Fail("Internetwork IP not found.");
			address = NekaiApp.LocalHost.AddressList.First();
		}

		string hostIp = address.MapToIPv4().ToString();
		return new(NekaiApp.Name, hostIp, port, Environment.ProcessId, protocol);
	}
}
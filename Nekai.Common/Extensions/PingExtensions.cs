using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Nekai.Common;

public static class PingExtensions {
	public static long ConnectTcp(this Ping ping, string hostname, int port, int timeOut = 4000)
	{
		NetEndPoint endPoint = new(hostname, port);
		return ping.ConnectTcp(endPoint, timeOut);
	}

	public static long ConnectTcp(this Ping ping, IPAddress host, int port, int timeOut = 4000)
		=> ping.ConnectTcp(new NetEndPoint(host, port), timeOut);

	public static long ConnectTcp(this Ping ping, NetEndPoint endPoint, int timeOut = 4000)
	{
		Stopwatch timer = new();
		using TcpClient client = new();
		client.SendTimeout = timeOut;
		if(endPoint.IPEndPointInfo is not null)
		{
			timer.Start();
			client.Connect(endPoint.IPEndPointInfo);
			timer.Stop();
		}else
		{
			timer.Start();
			client.Connect(endPoint.IPv4Address, endPoint.Port ?? 80);
			timer.Stop();
		}

		return timer.ElapsedMilliseconds >= timeOut
			? throw new TimeoutException()
			: timer.ElapsedMilliseconds;
	}
}

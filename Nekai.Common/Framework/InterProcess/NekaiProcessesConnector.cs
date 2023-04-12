using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Timers;
using Timer = System.Timers.Timer;

namespace Nekai.Common;

// TODO: Will probably need to implement an application-level reliable UDP broadcast protocol, since having a TCP connection
// open for each other application is overkill.

/// <summary>
/// Inter-process communication for Nekai-Framework-based applications.
/// </summary>
public class NekaiProcessesConnector
{
	/// <summary> How long the Timer waits between each call to the handler method, when active. </summary>
	private const int _TIMER_INTERVAL = 1000;
	/// <summary> Encoding of the transmitted messages. </summary>
	private static Encoding _Encoding => Encoding.Default;
	
	public ProtocolType Protocol;
	private Queue<byte[]> _Buffer { get; } = new();
	private Timer _Timer { get; }
	
	protected TcpClient Connection { get; }



	public NekaiProcessesConnector()
	{
		Connection = new();
		
		_Timer = new(_TIMER_INTERVAL)
		{
			Enabled = true
		};
		_Timer.Elapsed += _timer_Elapsed;
	}

	public void SendString(string data) {
		byte[] message = _Encoding.GetBytes(data);
		_Buffer.Enqueue(message);
		_AutoToggleTimer();
	}


	
	// Should be called whenever the buffer is updated.
	private void _AutoToggleTimer()
	{
		_Timer.Enabled = _Buffer.Count > 0;
	}
	
	// Needs to be async VOID due to restrictions on the Timer type.
	private async void _timer_Elapsed(object? sender, ElapsedEventArgs e)
	{
		if(_Buffer.Count > 0)
		{
			
		}

		_AutoToggleTimer();
	}

	private Result _TryParseMessage(byte[] message)
	{
		if(message.Length == 0)
			return Result.Failure($"{nameof(NekaiProcessesConnector)} received an empty message.");


		string data = _Encoding.GetString(message);

		return Result.Success();
	}

	private Result _TrySendMessage(byte[] message)
	{
		if(message.Length == 0)
			return Result.Failure($"{nameof(NekaiProcessesConnector)} attempted to send an empty message.");

		
		return Result.Success();
	}

	private int _CreateHash(string message)
	{
		return message.GetHashCode();
	}
}

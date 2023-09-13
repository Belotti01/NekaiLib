using System.Diagnostics;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using System.Text.Json.Serialization;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace Nekai.Common;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
[MemoryDiagnoser]
public class Program
{
	public static string[] _testStrings = Enumerable.Range(1, 300)
		.Select(x =>
		{
			return string.Create(50, (object?)null, (span, state) =>
			{
				for(int i = 0; i < span.Length; i++)
				{
					span[i] = Random.Shared.NextSingle() <= 0.5f
						? Random.Shared.NextChar() 
						: '\n';
				}
			});
		})
		.ToArray();

	public static async Task Main(string[] args)
	{
        // Do checks to ensure that the tests work
#if DEBUG
        await RunManualTestsAsync();
#else
		// Run the benchmarks
		RunBenchmarks();
#endif
	}

	public static async Task RunManualTestsAsync()
	{
		TcpListener listener = new(CurrentApp.LocalHost.IPAddress, 7142);
		Socket toServer = new(SocketType.Stream, ProtocolType.Tcp);

		listener.Start();
		await toServer.ConnectAsync(CurrentApp.LocalHost.IPAddress, 7142);
		Socket toClient = listener.AcceptSocket();

		_Test toSend = new(42, "Test Message");
		string json = JsonSerializer.Serialize(toSend);
		await toServer.SendJsonAsync(json, maxBufferSize: 4);
		
		_Test received = toClient.ReceiveJsonAsync<_Test>().Result.Value;
		Console.WriteLine($"Received:\n{received}");
	}

	[JsonSerializable(typeof(_Test))]
	public record _Test(int Value, string Message);

	public static void RunBenchmarks()
	{
		if(Debugger.IsAttached)
		{
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine("\nRun in RELEASE mode without debugger to start benchmarks.\n");
			Console.ResetColor();
			return;
		}

		BenchmarkRunner.Run<Program>();
	}

}

public class TestJson : ConfigurationFileManager<TestJson>
{
	[Configuration]
	public string TestConfig { get; set; } = "Test config value";
	[Configuration]
	public Color Color { get; set; } = Color.Red;

	public TestJson(string filepath)
		: base(filepath) { }
}
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously

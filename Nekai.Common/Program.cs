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
		NekaiConsole.PrintSignature();
		NekaiConsole.Write("Loading ", ConsoleColor.Red);

		CancellationTokenSource source = new();
		var task = NekaiConsole
			.CreateDotLoader()
			.WithCharacter('.')
			.WithColor(ConsoleColor.Yellow)
			.WithMaxCharacters(5)
			.RunAsync(source.Token);

		NekaiConsole.WriteLine();
		for(int i = 0; i < 10; i++)
		{
			await Task.Delay(TimeSpan.FromSeconds(1));
			NekaiConsole.Write($"{i} ");
		}
		source.Cancel();
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

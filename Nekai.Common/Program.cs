using System.Diagnostics;
using System.Net.Sockets;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Nekai.Common.Reflection;

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
		NekaiConsole.SlowPrintMode = true;

		NekaiLogs.Console.Error("Error");
		NekaiLogs.Console.Warning("Warning");
		NekaiLogs.Console.Information("Info");
		NekaiLogs.Console.Fatal("Fatal");
	}

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

	[Benchmark]
	public void SomethingToBenchmark()
	{
	}
}

#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
using System.Diagnostics;
using System.Drawing;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace Nekai.Common;

[MemoryDiagnoser]
public class Program
{


	public static async Task Main(string[] args)
	{
		// Do checks to ensure that the tests work
#if DEBUG
		RunTests();
#endif

		// Run the benchmarks
		RunBenchmarks();
	}

	public static void RunTests()
	{
		Program p = new();
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
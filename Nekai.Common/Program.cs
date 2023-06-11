using System.Diagnostics;
using System.Drawing;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace Nekai.Common;

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

	public static void Main(string[] args)
	{
		// Do checks to ensure that the tests work
#if DEBUG
		RunTests();
#else
		// Run the benchmarks
		RunBenchmarks();
#endif
	}

	public static void RunTests()
	{
		
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
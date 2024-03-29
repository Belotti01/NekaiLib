using System.Diagnostics;
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
		NekaiConsole.PrintSignature(2, ConsoleColor.Cyan);
        Console.Write("\t\t");
        var task = NekaiConsole.CreateDotLoader()
            .WithColor(ConsoleColor.Blue)
            .WithCharacter('~')
            .WithMaxCharacters(20)
            .RunAsync();

        var (even, odd) = Enumerable.Range(0, 100).Partition(x => x % 2 == 0);
        
        Console.WriteLine();
        Console.WriteLine(even.ToString(", "));
        Console.WriteLine(odd.ToString(' '));
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

		
	static string s = "TEST_LINE\nTEST_LINE_MAX\nTEST\nTEST2";
	[Benchmark]
    public int Linq()
	{
        return s
			.Split('\n')
			.MaxBy(x => x.Length)
			.Length;
    }

    [Benchmark]
    public int SpanManual()
    {
        var span = s.AsSpan();
        int currentLength = 0;
        int maxLength = -1;

        for(int i = 0; i < span.Length; i++)
        {
            if(span[i] == '\n')
            {
                if(currentLength > maxLength)
                {
                    maxLength = currentLength;
                }
                currentLength = 0;
                continue;
            }

            currentLength++;
        }

        if(currentLength > maxLength)
        {
            maxLength = currentLength;
        }

        return maxLength;
    }

    [Benchmark]
    public int Manual()
    {
        int currentLength = 0;
        int maxLength = -1;

        for(int i = 0; i < s.Length; i++)
        {
            if(s[i] == '\n')
            {
                if(currentLength > maxLength)
                {
                    maxLength = currentLength;
                }
                currentLength = 0;
                continue;
            }

            currentLength++;
        }

        if(currentLength > maxLength)
        {
            maxLength = currentLength;
        }

        return maxLength;
    }
}
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously

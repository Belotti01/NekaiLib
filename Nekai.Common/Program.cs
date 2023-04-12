using System.Drawing;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Attributes;
using System.Diagnostics;
using System.Net.Sockets;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;

namespace Nekai.Common;

[MemoryDiagnoser]
public class Program
{
	static readonly string[] _s = new[]
	{
		"1 ", "2  ", "3   ", "4"
	};
	private static readonly string[] _sLong = Enumerable.Range(1, 1000).Select(x => x.ToString()).ToArray();
	private static int[] arr = Array.Empty<int>();

	public static void Main(string[] args)
	{
		Console.WriteLine(NekaiGeneralConfiguration.Singleton.PreferDarkMode);
		NekaiGeneralConfiguration.Singleton.PreferDarkMode = false;
		var result = NekaiGeneralConfiguration.Singleton.TrySerialize();
		if(!result.IsSuccess)
		{
			NekaiLogs.Program.Error(result.Message);
			return;
		}
		Console.WriteLine(NekaiGeneralConfiguration.Singleton.PreferDarkMode);

		return;
		// Do checks to ensure that the tests work
		


		// Run the benchmarks
		RunBenchmarks();
	}

	public static void Init()
	{
		arr = File.ReadAllLines("C:\\Users\\39339\\Desktop\\soluzioni\\ASD-lab2\\in-40000.txt")
			.Where(x => !string.IsNullOrWhiteSpace(x))
			.Select(x => int.Parse(x))
			.ToArray();
	}

	private static void _QuickSort(ref int[] arr, int left, int right)
	{
		if(left >= right)
			return;

		int newPivot = left;
		int pivotIndex = (left + right) / 2;
		int pivot = arr[pivotIndex];
		(arr[right], arr[pivotIndex]) = (arr[pivotIndex], arr[right]);

		for(int i = left; i < right; i++)
		{
			if(arr[i] >= pivot)
				continue;
			(arr[i], arr[newPivot]) = (arr[newPivot], arr[i]);
			newPivot++;
		}

		(arr[right], arr[newPivot]) = (arr[newPivot], arr[right]);
		_QuickSort(ref arr, newPivot + 1, right);
		_QuickSort(ref arr, left, newPivot - 1);
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
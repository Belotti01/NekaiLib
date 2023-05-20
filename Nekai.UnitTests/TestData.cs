namespace Nekai.UnitTests;

public static class TestData
{
	public static readonly Dictionary<Type, Func<IEnumerable<object>>> Generators = new()
	{
		{ typeof(int), Int }, { typeof(string), String }
	};

	public static IEnumerable<T> PadRepeatTo<T>(IEnumerable<T> source, int finalSize)
	{
		int size = source.Count();
		var iterator = source.GetEnumerator();

		while(size++ < finalSize)
		{
			if(!iterator.MoveNext())
			{
				iterator.Reset();
			}
			source = source.Append(iterator.Current);
		}

		return source;
	}

	public static IEnumerable<TestCaseData> Generate(Type[] types)
	{
		IEnumerable<object>[] values = types
			.Select(t => Generators[t]())
			.ToArray();
		int maxSize = values.Max(v => v.Count());
		values = values.Select(v => PadRepeatTo(v, maxSize)).ToArray();

		IEnumerator<object>[] iterators = values
			.Select(v => v.GetEnumerator())
			.ToArray();

		TestCaseData data;
		for(int i = 0; i < maxSize; i++)
		{
			data = new();
			yield return new TestCaseData(values.Select(v => v.ElementAt(i)).ToArray());
		}
	}

	public static IEnumerable<object> Int()
	{
		return new object[]
		{
			3, 5, 1, 0, -1, -3, -5, 10_000, -10_000,
			int.MinValue, int.MaxValue
		}
		.Append(Enumerable.Range(0, 10).Select(i => Random.Shared.Next(-int.MaxValue, int.MaxValue)))
		.Distinct();
	}

	public static IEnumerable<object> Double()
	{
		return new object[]
		{
			3d, 5.2465d, 1d, 0, -1d, -3.2451d, -5d, 10_000d, -10_000d,
			double.MinValue, double.MaxValue, double.Pi, double.NegativeInfinity,
			double.NegativeZero, double.NaN, double.Epsilon
		}
		.Append(Enumerable.Range(0, 5).Select(i => Random.Shared.NextDouble() * Random.Shared.NextSign()));
	}

	public static IEnumerable<object> String()
	{
		return new object[]
		{
			"ABC", "012345", "", "     ", "\"", "{\\}", "Abc def ghijk lmno", "01, 45, 89",
			"_\t    \t_", "01.34.67", "01,34,67"
		}
		.Append(Enumerable.Range(20, 25).Select(i => Random.Shared.NextString(i)));
	}
}
namespace Nekai.Common;

public static class IndexFinderExtensions
{
	public static int[] IndexesWhere<T>(this IEnumerable<T> data, Func<T, bool> comparer)
		=> data._IndexesWhereInternal(comparer).ToArray();

	public static int[] IndexesOfAny<T>(this IEnumerable<T> data, IEnumerable<T> values, Func<T, T, bool> comparer)
	{
		int i = 0;
		List<int> indexes = new();

		foreach(var item in data)
		{
			if(values.Any(x => comparer.Invoke(x, item)))
				indexes.Add(i);
			i++;
		}
		return indexes.ToArray();
	}

	public static int[] IndexesOfAny<T>(this IEnumerable<T> data, Func<T, T, bool> comparer, params T[] values)
		=> data.IndexesOfAny(values, comparer);

	public static int[] IndexesOf<T>(this IEnumerable<T> data, T value) where T : IEquatable<T>
		=> data._IndexesOfInternal(value, (x, y) => x.Equals(y)).ToArray();

	public static int[] IndexesOfAny<T>(this IEnumerable<T> data, params T[] values) where T : IEquatable<T>
	{
		int index = 0;
		IEnumerable<int> indexes = Array.Empty<int>();

		foreach(T elem in data)
		{
			if(values.Contains(elem))
				indexes = indexes.Append(index);
			index++;
		}
		return indexes.ToArray();
	}

	public static int[] IndexesOfAny<T>(this IEnumerable<T> data, IEnumerable<T> values) where T : IEquatable<T>
		=> data._IndexesOfInternal(values, (x, y) => x.Equals(y)).ToArray();

	private static IEnumerable<int> _IndexesWhereInternal<T>(this IEnumerable<T> data, Func<T, bool> comparer)
	{
		int i = 0;

		foreach(var item in data)
		{
			if(comparer.Invoke(item))
				yield return i;
			++i;
		}
	}

	private static IEnumerable<int> _IndexesOfInternal<T>(this IEnumerable<T> data, T value, Func<T, T, bool> comparer)
	{
		int i = 0;

		foreach(var item in data)
		{
			if(comparer.Invoke(item, value))
				yield return i;
			++i;
		}
	}

	private static IEnumerable<int> _IndexesOfInternal<T>(this IEnumerable<T> data, IEnumerable<T> values, Func<T, T, bool> comparer)
	{
		int i = 0;

		foreach(var item in data)
		{
			if(values.Any(value => comparer.Invoke(item, value)))
				yield return i;
			++i;
		}
	}
}
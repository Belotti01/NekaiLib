namespace Nekai.Common;

public static class IndexesExtensions {

	public static int[] IndexesOf<T>(this IEnumerable<T> data, T value, Func<T, T, bool> comparer) {
		int i = 0;
		List<int> indexes = new();

		foreach(var item in data) {
			if(comparer.Invoke(item, value))
				indexes.Add(i);
			i++;
		}
		return indexes.ToArray();
	}

	public static int[] IndexesOfAny<T>(this IEnumerable<T> data, IEnumerable<T> values, Func<T, T, bool> comparer) {
		int i = 0;
		List<int> indexes = new();

		foreach(var item in data) {
			if(values.Any(x => comparer.Invoke(x, item)))
				indexes.Add(i);
			i++;
		}
		return indexes.ToArray();
	}

	public static int[] IndexesOfAny<T>(this IEnumerable<T> data, Func<T, T, bool> comparer, params T[] values)
		=> data.IndexesOfAny(values, comparer);
	
	public static int[] IndexesOf<T>(this IEnumerable<T> data, T value) where T : IEquatable<T>
		=> data.IndexesOf(value, (x, y) => x.Equals(y));

	public static int[] IndexesOfAny<T>(this IEnumerable<T> data, params T[] values) where T : IEquatable<T>
		=> data.IndexesOfAny(values);

	public static int[] IndexesOfAny<T>(this IEnumerable<T> data, IEnumerable<T> values) where T : IEquatable<T>
		=> data.IndexesOfAny(values, (x, y) => x.Equals(y));


}

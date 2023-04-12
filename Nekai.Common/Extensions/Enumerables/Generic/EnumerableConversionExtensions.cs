namespace Nekai.Common;

public static class EnumerableConversionExtensions
{
	public static SortedSet<T> ToSortedSet<T>(this IEnumerable<T> data)
		=> new(data);
}
namespace Nekai.Common;

public static class EnumerableConversionExtensions
{
	public static SortedSet<T> ToSortedSet<T>(this IEnumerable<T> data)
		=> new(data);

	public static ConcurrentList<T> ToConcurrentList<T>(this IEnumerable<T> data)
		=> new(data);
}
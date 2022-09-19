namespace Nekai.Common;
public static class DictionaryExtensions {

	public static SortedDictionary<TKey, TValue> ToSortedDictionary<TKey, TValue>(this IDictionary<TKey, TValue> data, IComparer<TKey>? comparer = null)
		where TKey : notnull
		=> new(data, comparer);
}

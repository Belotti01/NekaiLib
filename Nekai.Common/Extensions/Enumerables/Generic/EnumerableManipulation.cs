namespace Nekai.Common;

public static class EnumerableManipulation {
	/// <summary>
	/// Remove all instances of <see langword="null"/> from the collection.
	/// </summary>
	public static IEnumerable<T> ExceptNulls<T>(this IEnumerable<T?> data) where T : class {
		return data
			.Where(x => x is not null)!;
	}


}

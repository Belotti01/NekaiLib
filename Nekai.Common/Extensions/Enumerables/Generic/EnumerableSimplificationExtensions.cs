namespace Nekai.Common;

public static class EnumerableSimplificationExtensions
{
	/// <summary>
	/// Remove all instances of <see langword="null"/> from the collection.
	/// </summary>
	public static IEnumerable<T> ExceptNulls<T>(this IEnumerable<T?> data) where T : class
	{
		return data
			.Where(x => x is not null)!;
	}

	/// <summary>
	/// Remove all instances of <see langword="null"/> from the collection.
	/// </summary>
	public static IEnumerable<T> ExceptDefaults<T>(this IEnumerable<T> data) where T : struct, IEquatable<T>
	{
		return data
			.Where(x => !x.Equals(default));
	}
}
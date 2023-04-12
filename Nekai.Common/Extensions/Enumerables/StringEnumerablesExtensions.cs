namespace Nekai.Common;

public static class StringEnumerablesExtensions
{
	/// <summary>
	/// Exclude all instances of <see cref="string.Empty"/> from the collection.
	/// </summary>
	/// <param name="data"> The source collection. </param>
	public static IEnumerable<string> ExceptEmpty(this IEnumerable<string> data)
	{
		return data
			.Where(x => x.Length != 0);
	}

	/// <summary>
	/// Exclude all instances of <see cref="string.Empty"/> and whitespace-only <see langword="string"/>s from the collection.
	/// </summary>
	/// <param name="data"> The source collection. </param>
	public static IEnumerable<string> ExceptEmptyOrWhiteSpace(this IEnumerable<string?> data)
	{
		return data
			.Where(x => !string.IsNullOrWhiteSpace(x))!;
	}

	/// <summary>
	/// Remove all whitespace characters from the start and end of the strings in the collection.
	/// </summary>
	/// <param name="data"> The source collection. </param>
	public static IEnumerable<string> TrimAll(this IEnumerable<string> data)
	{
		return data
			.Select(x => x.Trim());
	}
}
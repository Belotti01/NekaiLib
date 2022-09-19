namespace Nekai.Common;

public static class StringEnumerablesCollections {
	public static IEnumerable<string> RemoveEmpty(this IEnumerable<string?> data) {
		return data
			.ExceptNulls()
			.Where(x => x.Length != 0);
	}

	public static IEnumerable<string> RemoveEmptyOrWhiteSpace(this IEnumerable<string?> data) {
		return data
			.Where(x => !string.IsNullOrWhiteSpace(x))!;
	}

	public static IEnumerable<string> TrimAll(this IEnumerable<string> data) {
		return data
			.Select(x => x.Trim());
	}
}

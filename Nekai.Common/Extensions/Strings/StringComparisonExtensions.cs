namespace Nekai.Common;

public static class StringComparisonExtensions
{
	// The default .NET comparer used by the overload string.Equals(string) applies Ordinal comparison, so keep the same behaviour
	// for the case-insensitive overloads by using the OrdinalIgnoreCase comparer by default.

	/// <summary>
	/// Shorthand for <see cref="string.Equals(string, string, StringComparison)"/> with <see cref="StringComparison.OrdinalIgnoreCase"/>.
	/// </summary>
	/// <inheritdoc cref="string.Equals(string?)"/>
	public static bool EqualsIgnoreCase(this string str, string value)
	{
		return str.Equals(value, StringComparison.OrdinalIgnoreCase);
	}

	/// <summary>
	/// Shorthand for <see cref="string.Equals(string, string, StringComparison)"/> with 
	/// <see cref="StringComparison.OrdinalIgnoreCase"/>, or <see cref="StringComparison.InvariantCultureIgnoreCase"/>
	/// if <paramref name="cultureInvariant"/> is set to <see langword="true"/>.
	/// </summary>
	/// <inheritdoc cref="string.Equals(string?)"/>
	public static bool EqualsIgnoreCase(this string str, string value, bool cultureInvariant)
	{
		return cultureInvariant
			? str.Equals(value, StringComparison.InvariantCultureIgnoreCase)
			: str.EqualsIgnoreCase(value);
	}
}
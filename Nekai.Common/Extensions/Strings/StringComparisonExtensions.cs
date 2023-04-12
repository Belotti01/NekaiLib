namespace Nekai.Common;

public static class StringComparisonExtensions
{
	public static bool EqualsIgnoreCase(this string str, string other)
	{
		return str.Equals(other, StringComparison.OrdinalIgnoreCase);
	}

	public static bool EqualsIgnoreCase(this string str, string other, bool cultureInvariant)
	{
		return cultureInvariant
			? str.Equals(other, StringComparison.InvariantCultureIgnoreCase)
			: str.EqualsIgnoreCase(other);
	}
}
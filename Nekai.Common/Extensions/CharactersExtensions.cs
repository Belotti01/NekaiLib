namespace Nekai.Common;

public static class CharactersExtensions
{
	public static bool Equals(this char c, char other, StringComparison comparison)
	{
		return comparison switch
		{
			StringComparison.Ordinal => c.Equals(other),
			StringComparison.OrdinalIgnoreCase => char.ToLower(c).Equals(char.ToLower(other)),
			StringComparison.InvariantCultureIgnoreCase => c.ToLowerInvariant().Equals(other.ToLowerInvariant()),
			// No fast way to check - convert to string
			_ => c.ToString().Equals(other.ToString(), comparison),
		};
	}

	public static char ToUpper(this char c)
		=> char.ToUpper(c);

	public static char ToUpperInvariant(this char c)
		=> char.ToUpperInvariant(c);

	public static char ToLower(this char c)
		=> char.ToLower(c);

	public static char ToLowerInvariant(this char c)
		=> char.ToLowerInvariant(c);

	public static bool IsNumber(this char c)
		=> char.IsNumber(c);

	public static bool IsUpper(this char c)
		=> char.IsUpper(c);

	public static bool IsLower(this char c)
		=> char.IsLower(c);
}
namespace Nekai.Common;

public static class CharactersExtensions
{
	/// <summary>
	/// Compare two characters for equality using the specified <see cref="StringComparison"/>.
	/// </summary>
	/// <param name="c">The first character.</param>
	/// <param name="other">The character to compare to.</param>
	/// <param name="comparison">The comparison method.</param>
	/// <returns><see langword="true"/> if the two characters are equal. <see langword="false"/> otherwise. </returns>
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

	/// <inheritdoc cref="char.ToUpper(char)"/>
	public static char ToUpper(this char c)
		=> char.ToUpper(c);

	/// <inheritdoc cref="char.ToUpperInvariant(char)"/>
	public static char ToUpperInvariant(this char c)
		=> char.ToUpperInvariant(c);

	/// <inheritdoc cref="char.ToLower(char)"/>
	public static char ToLower(this char c)
		=> char.ToLower(c);

	/// <inheritdoc cref="char.ToLowerInvariant(char)"/>
	public static char ToLowerInvariant(this char c)
		=> char.ToLowerInvariant(c);

	/// <inheritdoc cref="char.IsNumber(char)"/>
	public static bool IsNumber(this char c)
		=> char.IsNumber(c);

	/// <inheritdoc cref="char.IsUpper(char)"/>
	public static bool IsUpper(this char c)
		=> char.IsUpper(c);

	/// <inheritdoc cref="char.IsLower(char)"/>
	public static bool IsLower(this char c)
		=> char.IsLower(c);
}
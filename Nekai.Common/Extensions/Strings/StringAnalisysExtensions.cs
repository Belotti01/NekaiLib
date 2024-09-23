namespace Nekai.Common;

public static class StringAnalisysExtensions
{
	/// <summary>
	/// Check whether the <see cref="ReadOnlySpan{T}"/> only contains the characters
	/// in <paramref name="characters"/>.
	/// </summary>
	/// <param name="str"> The span to check. </param>
	/// <param name="characters"> The characters to find in the <paramref name="str"/>. </param>
	/// <param name="comparison"> The comparer to use. </param>
	/// <returns>
	/// <see langword="true"/> if the <paramref name="str"/> only contains characters among the
	/// ones defined in <paramref name="characters"/>; <see langword="false"/> otherwise.
	/// </returns>
	public static bool OnlyContains(this ReadOnlySpan<char> str, char[] characters, StringComparison comparison = StringComparison.Ordinal)
	{
		return str.All(c => characters.Any(x => x.Equals(c, comparison)));
	}

	/// <inheritdoc cref="ContainsAll(ReadOnlySpan{char}, char[], StringComparison)"/>
	public static bool ContainsAll(this ReadOnlySpan<char> str, params char[] characters)
		=> str.ContainsAll(characters, StringComparison.Ordinal);

	/// <summary>
	/// Check whether a <see cref="ReadOnlySpan{T}"/> contains all the specified <paramref name="characters"/>.
	/// </summary>
	/// <param name="str">The span to check.</param>
	/// <param name="characters">The characters to find in <paramref name="str"/>.</param>
	/// <param name="comparisonType">The comparer to use.</param>
	/// <returns><see langword="true"/> if the <paramref name="str"/> contains all the specified <paramref name="characters"/>;
	/// <see langword="false"/> otherwise.</returns>
	public static bool ContainsAll(this ReadOnlySpan<char> str, char[] characters, StringComparison comparisonType = StringComparison.Ordinal)
	{
		str.All(c => characters.Any(x => x.Equals(c, comparisonType)));
		for(int i = str.Length - 1; i >= 0; i--)
		{
			if(!str.Contains(characters[i], comparisonType))
				return false;
		}
		return true;
	}

	/// <summary>
	/// Check whether a <see cref="ReadOnlySpan{T}"/> contains the specified <paramref name="value"/>.
	/// </summary>
	/// <param name="str">The span to check.</param>
	/// <param name="value">The character to find in <paramref name="str"/>.</param>
	/// <param name="comparisonType">The comparer to use.</param>
	/// <returns><see langword="true"/> if the <paramref name="str"/> contains at least one instance of the specified
	/// <paramref name="value"/>; <see langword="false"/> otherwise.</returns>
	public static bool Contains(this ReadOnlySpan<char> str, char value, StringComparison comparisonType = StringComparison.Ordinal)
		=> str.IndexOf(value, comparisonType) >= 0;

	/// <inheritdoc cref="string.IndexOf(char, StringComparison)"/>
	public static int IndexOf(this ReadOnlySpan<char> str, char value, StringComparison comparisonType = StringComparison.Ordinal)
	{
		for(int i = str.Length - 1; i >= 0; --i)
		{
			if(str[i].Equals(value, comparisonType))
				return i;
		}
		return -1;
	}

	/// <summary>
	/// Whether the whole <see langword="string"/> can be parsed into a numeric value.
	/// </summary>
	/// <param name="str"> The <see langword="string"/> to parse. </param>
	public static bool IsNumeric(this ReadOnlySpan<char> str)
	{
		int dotCount = ReadOnlySpanExtensions.Count(str, '.');

		return dotCount switch
		{
			0 => str.IsInteger(),
			1 => str.All(x => char.IsNumber(x) || x == '.'),
			_ => false
		};
	}


	/// <summary>
	/// Whether the whole <see langword="string"/> can be parsed into an integer value.
	/// </summary>
	/// <param name="str"> The <see langword="string"/> to parse. </param>
	public static bool IsInteger(this ReadOnlySpan<char> str)
	{
		return str.All(char.IsNumber);
	}
}
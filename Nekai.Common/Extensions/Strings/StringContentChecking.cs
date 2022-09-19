namespace Nekai.Common;

public static class StringContentChecking {
	/// <inheritdoc cref="OnlyContains(string, IEnumerable{char}, StringComparison)"/>
	public static bool OnlyContains(this string str, StringComparison comparison, params char[] characters)
		=> str.OnlyContains(characters.AsEnumerable(), comparison);

	/// <inheritdoc cref="OnlyContains(string, IEnumerable{char}, StringComparison)"/>
	public static bool OnlyContains(this string str, params char[] characters)
		=> str.OnlyContains(characters.AsEnumerable());

	/// <summary>
	/// Check whether the <see langword="string"/> only contains the characters
	/// in <paramref name="characters"/>.
	/// </summary>
	/// <param name="str"> The <see langword="string"/> to check. </param>
	/// <param name="characters"> The characters to find in the <paramref name="str"/>. </param>
	/// <returns>
	/// <see langword="true"/> if the <paramref name="str"/> only contains characters among the 
	/// ones defined in <paramref name="characters"/>; <see langword="false"/> otherwise.
	/// </returns>
	public static bool OnlyContains(this string str, IEnumerable<char> characters, StringComparison comparison = StringComparison.Ordinal) {
		return str.All(c => characters.Any(x => x.Equals(c, comparison)));
	}


	/// <inheritdoc cref="ContainsAll(string, IEnumerable{char}, StringComparison)"/>
	public static bool ContainsAll(this string str, params char[] characters)
		=> str.ContainsAll(characters.AsEnumerable());

	/// <inheritdoc cref="ContainsAll(string, IEnumerable{char}, StringComparison)"/>
	public static bool ContainsAll(this string str, StringComparison comparison, params char[] characters)
		=> str.ContainsAll(characters.AsEnumerable(), comparison);

	/// <summary>
	/// Check whether a <see langword="string"/> contains all the specified <paramref name="characters"/>.
	/// </summary>
	/// <param name="str">The string to check.</param>
	/// <param name="characters">The characters to find in <paramref name="str"/>.</param>
	/// <param name="comparison">The comparer to use.</param>
	/// <returns><see langword="true"/> if the <paramref name="str"/> contains all the specified <paramref name="characters"/>;
	/// <see langword="false"/> otherwise.</returns>
	public static bool ContainsAll(this string str, IEnumerable<char> characters, StringComparison comparison = StringComparison.Ordinal) {
		return characters.All(c => str.Contains(c, comparison));
	}

	public static bool IsNumeric(this string str) {
		return decimal.TryParse(str, out _);
	}

	public static bool IsInteger(this string str) {
		return int.TryParse(str, out _);
	}
}

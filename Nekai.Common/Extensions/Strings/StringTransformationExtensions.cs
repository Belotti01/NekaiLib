using System.Globalization;

namespace Nekai.Common;

public static class StringTransformationExtensions
{
	/// <inheritdoc cref="ToTitleCase(string, CultureInfo)"/>
	public static string ToTitleCase(this string str)
		=> str.ToTitleCase(CultureInfo.CurrentCulture);

	/// <inheritdoc cref="TextInfo.ToTitleCase(string)"/>
	/// <param name="str"> The string to convert. </param>
	/// <param name="culture"> The Culture information to use for the conversion. </param>
	public static string ToTitleCase(this string str, CultureInfo culture)
		=> culture.TextInfo.ToTitleCase(str);

	/// <inheritdoc cref="ToLength(string, int, char, bool)"/>
	public static string ToLength(this string str, int length, bool padLeft = false)
		=> ToLength(str, length, ' ', padLeft);

	/// <summary>
	/// Returns the <paramref name="str"/> truncated or padded to the specified <paramref name="length"/>.
	/// </summary>
	/// <param name="str"> The starting string. </param>
	/// <param name="length"> The final length of the string. </param>
	/// <param name="paddingChar"> The <see langword="char"/> used to pad the <paramref name="str"/>. </param>
	/// <param name="padLeft"> Whether to apply left padding instead of right padding. </param>
	/// <returns> The original <see langword="string"/> if its length equals <paramref name="length"/>;
	/// <paramref name="str"/> truncated to length <paramref name="length"/> if its length is bigger than
	/// <paramref name="length"/>;
	/// <paramref name="str"/> padded (based on <paramref name="padLeft"/>) to length <paramref name="length"/> if its
	/// length is smaller than <paramref name="length"/>.
	/// </returns>
	public static string ToLength(this string str, int length, char paddingChar, bool padLeft = false)
	{
		if(str.Length == length)
			return str;

		if(str.Length > length)
			return str[..length];

		return padLeft
			? str.PadLeft(length, paddingChar)
			: str.PadRight(length, paddingChar);
	}
}
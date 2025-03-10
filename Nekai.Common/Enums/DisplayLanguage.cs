﻿namespace Nekai.Common;

/// <summary>
/// Enum that lists all languages supported by the Nekai framework.
/// </summary>
public enum DisplayLanguage
{
	Default,
	EnglishUsa,
	EnglishUk,
	Italian
}

public static class DisplayLanguageExtensions
{
	/// <summary>
	/// Get the ISO 639-2 lower-case three-letter code for the <paramref name="language"/>.
	/// </summary>
	/// <param name="language"> The language to convert. </param>
	/// <exception cref="ArgumentOutOfRangeException"> Language is not supported. </exception>
	public static string ToThreeLetterISOName(this DisplayLanguage language)
	{
		return language switch
		{
			DisplayLanguage.Default => "eng",
			DisplayLanguage.Italian => "ita",
			DisplayLanguage.EnglishUsa => "eng",
			DisplayLanguage.EnglishUk => "eng",
			_ => throw new ArgumentOutOfRangeException(nameof(language), language, "The selected language is not supported.")
		};
	}
}
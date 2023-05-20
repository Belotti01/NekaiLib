namespace Nekai.Common;

public static class NumberInfoExtensions
{
	private const int _INT_MAX_VALUE_DIGITS = 10;
	private const int _LONG_MAX_VALUE_DIGITS = 19;

	/// <summary>
	/// Returns the amount of digits of an integer.
	/// </summary>
	/// <param name="number"> The number to count the digits of. </param>
	/// <returns> The amount of digits of the decimal base form of the <paramref name="number"/>, not counting the
	/// sign of negative values. </returns>
	public static int CountDigits(this int number)
	{
		// Handle the values in range [0, 9] together with the edge cases.
		if(number < 10)
		{
			if(number > -10)
				return 1;
			// Avoid overflows when converting to positive.
			if(number == int.MinValue)
				return _INT_MAX_VALUE_DIGITS;

			number = -number;
		}

		return (int)Math.Floor(Math.Log10(number) + 1);
	}

	public static int CountDigits(this long number)
	{
		// Handle the values in range [0, 9] together with the edge cases.
		if(number < 10)
		{
			if(number > -10)
				return 1;
			// Avoid overflows when converting to positive.
			if(number == long.MinValue)
				return _LONG_MAX_VALUE_DIGITS;

			number = -number;
		}

		return (int)Math.Floor(Math.Log10(number) + 1);
	}
}
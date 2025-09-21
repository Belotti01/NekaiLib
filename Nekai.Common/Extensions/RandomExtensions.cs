namespace Nekai.Common;

public static class RandomExtensions
{
	/// <summary>
	/// Generate a random character.
	/// </summary>
	/// <inheritdoc cref="NextChar(Random, char, char)"/>
	public static char NextChar(this Random random)
		=> random.NextChar(char.MinValue, char.MaxValue);
	
	/// <summary>
	/// Generate a random character of value lower or equal to <paramref name="max"/>.
	/// </summary>
	/// <inheritdoc cref="NextChar(Random, char, char)"/>
	public static char NextChar(this Random random, char max)
		=> random.NextChar(char.MinValue, max);

	/// <summary>
	/// Generate a random character between the specified <paramref name="min"/> and
	/// <paramref name="max"/> values.
	/// </summary>
	/// <param name="random"> The <see cref="Random"/> object to use. </param>
	/// <param name="min"> The inclusive lower bound of the generated <see cref="char"/>. </param>
	/// <param name="max"> The inclusive upper bound of the generated <see cref="char"/>. </param>
	/// <returns>A random <see cref="char"/> within the <paramref name="min"/> to <paramref name="max"/> range (inclusive). </returns>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="min"/> is greater than <paramref name="max"/>, or either parameter is not a valid <see cref="char"/> value.</exception>
	public static char NextChar(this Random random, char min, char max)
	{
		return (char)random.Next(min, max);
	}

	/// <summary>
	/// Generate a random string of length within the range <paramref name="minLength"/> to
	/// <paramref name="maxLength"/> (inclusive).
	/// </summary>
	/// <param name="random"> The <see cref="Random"/> object to use. </param>
	/// <param name="minLength"> The minimum length of the generated <see cref="string"/>. </param>
	/// <param name="maxLength"> The maximum length of the generated <see cref="string"/>. </param>
	/// <returns> A <see cref="string"/> of random length within the inclusive bounds <paramref name="minLength"/>
	/// and <paramref name="maxLength"/>. </returns>
	/// <exception cref="ArgumentOutOfRangeException"> <paramref name="minLength"/> or <paramref name="maxLength"/> is
	/// less than 0, or <paramref name="minLength"/> is greater than <paramref name="maxLength"/>. </exception>
	public static string NextString(this Random random, int minLength, int maxLength)
	{
		if(minLength < 0)
			throw new ArgumentOutOfRangeException(nameof(minLength), minLength, $"{nameof(minLength)} must be greater than or equal to 0");
		if(maxLength < 0)
			throw new ArgumentOutOfRangeException(nameof(maxLength), minLength, $"{nameof(maxLength)} must be greater than or equal to 0");
		if(minLength > maxLength)
			throw new ArgumentOutOfRangeException(nameof(minLength), minLength, $"{nameof(minLength)} must be less than or equal to {nameof(maxLength)}");

		int length = random.Next(minLength, maxLength);
		Span<char> chars = NekaiMemory.IsStackallocSafe(length, sizeof(char))
			? stackalloc char[length]
			: new char[length];

		for(int i = 0; i < length; i++)
		{
			chars[i] = random.NextChar();
		}
		return new string(chars);
	}

	/// <summary>
	/// Generate a random string of length <paramref name="length"/>.
	/// </summary>
	/// <param name="random"> The <see cref="Random"/> object to use. </param>
	/// <param name="length"> The length of the generated <see cref="string"/>. </param>
	/// <returns> A randomly generated <see cref="string"/> of the specified <paramref name="length"/>. </returns>
	/// <exception cref="ArgumentOutOfRangeException"> <paramref name="length"/> is less than 0. </exception>
	public static string NextString(this Random random, int length)
	{
		if(length < 0)
			throw new ArgumentOutOfRangeException(nameof(length), length, $"{nameof(length)} must be greater than or equal to 0");
		if(length == 0)
			return "";

		Span<char> chars = NekaiMemory.IsStackallocSafe(length, sizeof(char))
			? stackalloc char[length]
			: new char[length];

		for(int i = length - 1; i >= 0; --i)
		{
			chars[i] = random.NextChar();
		}

		return new string(chars);
	}

	/// <summary>
	/// Randomly generate a value that is either 1 or -1.
	/// </summary>
	/// <param name="random"> The <see cref="Random"/> object to use. </param>
	/// <returns> An <see langword="int"/> reppresentation of either 1 or -1. </returns>
	public static int NextSign(this Random random)
		=> random.Next(0, 2) == 0 ? -1 : 1;
}
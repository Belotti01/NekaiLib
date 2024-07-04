namespace Nekai.Common;

public static class NekaiString
{
	/// <summary> The (multiline) signature of the Nekai framework.</summary>
	/// <remarks> To properly print the signature to the Console, use <see cref="NekaiConsole.PrintSignature(int, ConsoleColor, ConsoleColor)"/>. </remarks>
	public static string Signature => NekaiResources.Signature;

	/// <summary>
	/// Generate a progress bar string displaying the <paramref name="progressPercentage"/> trimmed to
	/// <paramref name="barSteps"/> steps.
	/// </summary>
	/// <param name="progressPercentage"> The percentage of the bar to be filled. Must be in the range 0 to 100 (inclusive). </param>
	/// <param name="barSteps"> The total amount of steps to display in the progress bar. </param>
	/// <param name="filledChar"> The <see langword="char"/> identifying a filled step of the bar. </param>
	/// <param name="emptyChar">  The <see langword="char"/> identifying an empty step of the bar. </param>
	/// <param name="prefix"> <see langword="string"/> to place before the progress bar. </param>
	/// <param name="postfix"> <see langword="string"/> to be appended to the progress bar. </param>
	/// <returns>
	/// A <see langword="string"/> starting with <paramref name="prefix"/> and ending with <paramref name="postfix"/>,
	/// containing <paramref name="barSteps"/> amount of <paramref name="filledChar"/> and <paramref name="emptyChar"/>
	/// based on the <paramref name="progressPercentage"/>.
	/// </returns>
	/// <exception cref="ArgumentException"> Thrown when <paramref name="barSteps"/> is a negative value, or
	/// <paramref name="progressPercentage"/> is not within the range [0, 100]. </exception>
	public static string CreateProgressBar(int progressPercentage, int barSteps, char filledChar = '=', char emptyChar = ' ', string prefix = "[", string postfix = "]")
	{
		// tl;dr of the method is:
		// return prefix + new string(filledChar, <filled steps>) + new string(emptyChar, <empty steps>) + postfix;
		if(barSteps <= 0)
			throw new ArgumentException("Progress bar length must be greater than 0.", nameof(barSteps));
		if(progressPercentage is < 0 or > 100)
			throw new ArgumentException("Progress percentage must be in the range 0 to 100 (inclusive).", nameof(progressPercentage));

		int length = barSteps + prefix.Length + postfix.Length;

		// May be overkill to use string.Create. Didn't run benchmarks. Ignore it. It's just a string building operation with
		// (supposedly) minimal memory allocation.
		string progressString = string.Create(length, 0, (span, state) =>
		{
			int nextStep = prefix.Length;
			int i = 0;
			// Prefix
			while(i < nextStep)
			{
				span[i] = prefix[i];
				i++;
			}
			// Filled cells
			nextStep += (int)(progressPercentage / 100f * barSteps);    // Rounded down
			while(i < nextStep)
			{
				span[i++] = filledChar;
			}
			// Empty cells
			nextStep = span.Length - postfix.Length;
			while(i < nextStep)
			{
				span[i++] = emptyChar;
			}
			// Postfix
			for(i = 0; i < postfix.Length; i++)
			{
				span[^(i + 1)] = postfix[i];
			}
		});

		return progressString;
	}
}
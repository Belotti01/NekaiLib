namespace Nekai.Common;

public static class StringManipulationExtensions
{
	private static readonly string[] _NewLineCharacters = new[] { "\n", "\r\n" };

	public static string[] SplitBySpaces(this string str)
	{
		return str.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);
	}

	public static string[] SplitLines(this string str, StringSplitOptions options = StringSplitOptions.None)
	{
		return str.Split(_NewLineCharacters, options);
	}

	public static ArgumentsReader ToParameters(this string str, char parameterPrefix = ArgumentsReader.DEFAULT_PARAMETER_PREFIX)
	{
		return new(str, parameterPrefix);
	}

	public static ArgumentsReader ToParameters(this IEnumerable<string> args, char parameterPrefix = ArgumentsReader.DEFAULT_PARAMETER_PREFIX)
	{
		return new(args, parameterPrefix);
	}
}
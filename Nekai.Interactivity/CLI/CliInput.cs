using System.Text;

namespace Nekai.Interactivity;

// Contains methods for both Output and Input.
// Output methods can be found in: CLI/CliOutput.cs
public static partial class Cli
{
	private static readonly string[] _yesAliases = { "TRUE", "YES", "Y", "T", "YEP", "YEAH", "YUP", "AFFIRMATIVE" };
	private static readonly string[] _noAliases = { "FALSE", "NO", "N", "F", "NOPE", "NEGATIVE" };

	public static T Request<T>(string? message, T min, T max) where T : IParsable<T>, IComparisonOperators<T, T, bool>
	{
		return _Request(message,
			x => T.Parse(x, null),
			x => T.TryParse(x, null, out _) ? null : "A numeric value is expected.",
			x => x < min || x > max ? $"Value must be in range {min} to {max}." : null
		);
	}

	public static T Request<T>(string? message, T max) where T : IParsable<T>, IComparisonOperators<T, T, bool>
	{
		return _Request(message,
			x => T.Parse(x, null),
			x => T.TryParse(x, null, out _) ? null : "A numeric value is expected.",
			x => x > max ? $"Value must be below or equal to {max}." : null
		);
	}

	public static T Request<T>(string? message) where T : IParsable<T>
	{
		return _Request(message,
			x => T.Parse(x, null),
			x => T.TryParse(x, null, out _) ? null : "Insert a valid value."
		);
	}

	public static string RequestLine(string? requestMessage, int maxLength = int.MaxValue)
		=> RequestLine(requestMessage, 0, maxLength);

	public static string RequestLine(string? requestMessage, int minLength, int maxLength)
	{
		return _Request(requestMessage,
			x => x,
			x => x.Length < minLength || x.Length > maxLength
				? minLength == maxLength
					? $"Text must be {minLength} character(s) long."
					: maxLength == int.MaxValue
						? $"Text must be at least {minLength} character(s) long."
						: $"Length must be in range {minLength} to {maxLength}."
				: null
		);
	}

	public static bool TryReadYesNo(out bool input)
	{
		string args = Console.ReadLine()?.Trim().ToUpper() ?? "";

		if(input = _yesAliases.Contains(args))
			return true;
		input = false;
		return _noAliases.Contains(args);
	}

	public static bool ReadYesNo(bool repeatUntilValid = true)
	{
		if(!repeatUntilValid)
		{
			string args = Console.ReadLine()?.Trim().ToUpper() ?? "";
			// Always fallback to "No"
			return _yesAliases.Contains(args);
		}

		bool input;
		while(!TryReadYesNo(out input))
		{
			DeleteLine();
		}
		return input;
	}

	private static int _PrintRequest(string? requestMessage, string? errorMessage = null, int minimumDivLength = 10)
	{
		int outputLines = 3;
		int divLength =
			requestMessage is null
				? errorMessage is null
					? 0
					: errorMessage.Length
				: errorMessage is null
					? requestMessage.Length
					: Math.Max(requestMessage.Length, errorMessage.Length);
		divLength = Math.Max(minimumDivLength, divLength + 2);
		WriteDivisor(divLength);

		if(!string.IsNullOrWhiteSpace(errorMessage))
		{
			errorMessage = errorMessage.Trim();
			WriteLine(errorMessage, Configuration.ErrorColor);
			outputLines += errorMessage.Count(x => x == '\n') + 1;
		}

		if(!string.IsNullOrWhiteSpace(requestMessage))
		{
			requestMessage = requestMessage.Trim();
			WriteLine(requestMessage, Configuration.RequestMessageColor);
			outputLines += requestMessage.Count(x => x == '\n') + 1;
		}

		Console.CursorTop += 1;
		WriteDivisor(divLength);
		Console.CursorTop -= 2;
		Write("> ", Configuration.RequestMessageColor);
		return outputLines;
	}

	private static T _Request<T>(string? requestMessage, Func<string, T> converter, Func<string, string?>? rawValidator = null, Func<T, string?>? validator = null)
	{
		string? errorMessage = null;
		string? rawInput;
		int outputLines;
		bool verified = false;
		T value;

		do
		{
			outputLines = _PrintRequest(requestMessage, errorMessage);
			rawInput = Console.ReadLine();
			Console.CursorTop += 2;

			if(rawInput is null)
				continue;

			if(rawValidator is not null)
			{
				errorMessage = rawValidator(rawInput);
				if(errorMessage is not null)
					continue;
			}

			value = converter(rawInput);

			if(validator is not null)
				errorMessage = validator(value);
			verified = errorMessage is null;

			if(!verified || Configuration.DeleteRequestAfterValidInput)
			{
				DeleteLines(outputLines + 1, false);
			}
			if(verified)
				return value;
		} while(!verified);

		Exceptor.ThrowAndLogError("Internal Error: Input Request interrupted without valid value.");
		return default!;
	}

	public static T RequestChoice<T>(string? requestMessage, IDictionary<string, T> options) 
	{
		string choice = RequestChoice(requestMessage, options.Keys);
		return options[choice];
	}

	public static string RequestChoice(string? requestMessage, params string[] options)
		=> RequestChoice(requestMessage, options.AsEnumerable());

	public static string RequestChoice(string? requestMessage, IEnumerable<string> options)
	{
		int optionsCount = options.Count();
		if(optionsCount == 0)
			Exceptor.ThrowAndLogError(new ArgumentException("No options were provided.", nameof(options)));
		else if(options.Distinct(StringComparer.OrdinalIgnoreCase).Count() != optionsCount)
			Exceptor.ThrowAndLogError(new ArgumentException("Duplicate options were provided.", nameof(options)));

		Dictionary<string, string> mappedOptions = new(optionsCount);
		int index = 0;
		foreach(string option in options)
		{
			mappedOptions.Add((index++).ToString(), option);
		}

		StringBuilder requestBuilder = new(requestMessage);
		if(requestBuilder.Length > 0)
			requestBuilder.AppendLine();
		foreach(var option in mappedOptions)
		{
			requestBuilder.AppendLine($"\t{option.Key}: {option.Value}");
		}
		requestMessage = requestBuilder.ToString();

		string input;
		string? optionEquivalent;
		do
		{
			input = RequestLine(requestMessage).Trim();
			if(mappedOptions.TryGetValue(input, out string? value))
				return value;

			// Compare the input to the string option values, and keep the casing of the OPTION rather than the input's
			optionEquivalent = options.FirstOrDefault(x => x.Equals(input, StringComparison.OrdinalIgnoreCase));
			if(optionEquivalent is not null)
				return optionEquivalent;
		} while(true);
	}
}
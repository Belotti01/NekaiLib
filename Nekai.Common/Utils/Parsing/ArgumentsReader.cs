using System.Diagnostics.CodeAnalysis;

namespace Nekai.Common;

public class ArgumentsReader
{
	internal const char DEFAULT_PARAMETER_PREFIX = '-';
	protected Dictionary<string, string> _arguments = new(StringComparer.OrdinalIgnoreCase);

	public string ParameterlessArgument => _arguments[""];
	public char ParameterPrefix { get; protected set; }
	public string? this[string parameter] => _arguments.GetValueOrDefault(parameter);
	public string? this[params string[] parameters] => this[parameters.AsEnumerable()];
	public string? this[IEnumerable<string> parameters] => ReadAny(parameters);

	public ArgumentsReader(IEnumerable<string> args, char parameterPrefix = DEFAULT_PARAMETER_PREFIX)
	{
		ParameterPrefix = parameterPrefix;
		// Pre-allocate for the whole text + one " character for each argument (which is meant to be two for each value)
		// + another for each argument -1 to include the spaces between them + one space at the end to avoid useless checks
		StringBuilder sb = new(args.Sum(x => x.Length) + args.Count() * 2);

		foreach(string arg in args)
		{
			if(arg.StartsWith(ParameterPrefix))
			{
				sb.Append(arg).Append(' ');
			}
			else
			{
				// If the string doesn't start with the parameter prefix, it's a parameterless argument
				sb.Append('"').Append(arg).Append('"');
			}
			sb.Append(' ');
		}

		NewParse(sb.ToString());
	}

	public ArgumentsReader(string args, char parameterPrefix = DEFAULT_PARAMETER_PREFIX)
	{
		ParameterPrefix = parameterPrefix;
		NewParse(args);
	}

	public bool TryRead(string parameter, [NotNullWhen(true)] out string? value)
	{
		value = this[parameter];
		return value is not null;
	}

	public string? Read(string? parameter)
	{
		return _arguments.GetValueOrDefault(parameter ?? "");
	}

	public string Read(string? parameter, string defaultValue)
	{
		parameter ??= "";
		return _arguments.ContainsKey(parameter)
			? _arguments[parameter]
			: defaultValue;
	}

	public string? ReadAny(params string?[] parameters)
		=> ReadAny(parameters.AsEnumerable());

	public string? ReadAny(IEnumerable<string?> parameters, string? defaultValue = null)
	{
		foreach(var parameter in parameters)
		{
			if(TryRead(parameter ?? "", out string? value))
				return value;
		}

		return defaultValue;
	}

	public bool TryReadAny(IEnumerable<string?> parameters, [NotNullWhen(true)] out string? value)
	{
		value = ReadAny(parameters);
		return value is not null;
	}

	protected void NewParse(string args)
	{
		char? wrapper;
		Dictionary<string, string> parameters = new();
		args = args.Trim();
		var span = args.AsSpan();
		string? lastKey = null, lastValue = null;
		int endIndex;

		for(int i = 0; i < args.Length; i++)
		{
			if(span[i] == ParameterPrefix)
			{
				endIndex = i;
				do
				{
					++endIndex;
				} while(endIndex < span.Length && span[endIndex] is not ' ' or '=');

				if(lastKey is not null && lastValue is null)
					parameters.Add(lastKey, "");

				lastKey = ParseKey(span[i..endIndex]);
				i = endIndex + 1;

				if(i >= span.Length)
					break;
			}

			if(lastKey is not null)
			{
				// Make sure to skip eventual additional spaces
				while(i < span.Length && span[i] == ' ' && span[i + 1] is ' ' or '"' or '\'')
				{
					i++;
				}

				wrapper = span[i];
				endIndex = i;
				do
				{
					++endIndex;
				} while(endIndex < span.Length && span[endIndex] != wrapper.Value || span[endIndex - 1] == '\\');
				lastValue = ParseValue(span[i..endIndex], wrapper.Value);

				if(parameters.ContainsKey(lastKey))
					parameters[lastKey] = lastValue;
				else
					parameters.Add(lastKey ?? "", lastValue);
				lastKey = null;
				lastValue = null;
				i = endIndex;
				continue;
			}
		}

		if(lastKey is not null)
		{
			parameters.Add(lastKey, "");
		}
		else if(lastValue is not null)
		{
			parameters.Add("", lastValue);
		}

		_arguments = parameters;
	}

	private string ParseKey(ReadOnlySpan<char> part)
	{
		return part[1..].ToString();
	}

	private string ParseValue(ReadOnlySpan<char> part, char wrapper)
	{
		return part[1..].ToString();
	}

	protected void Parse(string args)
	{
		NewParse(args);
		return;

		char[] parameterPostfixes = new[] { '\'', '"', ' ' };

		bool inQuotes = false;
		bool inDoubleQuotes = false;
		bool preceedingBackslash = false;
		string parameter = "";
		char c;
		StringBuilder sb = new();
		_arguments.Clear();

		for(int i = 0; i < args.Length; i++)
		{
			c = args[i];

			// Check for quotes end
			if(!preceedingBackslash)
			{
				if(inQuotes && c == '\'')
				{
					inQuotes = false;
					continue;
				}
				if(inDoubleQuotes && c == '"')
				{
					inDoubleQuotes = false;
					continue;
				}
			}

			if(inQuotes || inDoubleQuotes)
			{
				if(preceedingBackslash)
				{
					// Escape character
					sb.Append(c);
					preceedingBackslash = false;
					continue;
				}
				if(c == '\\')
				{
					// Escape the next character
					preceedingBackslash = true;
					continue;
				}
			}
			else if(sb.ToString().Trim().Length == 0)
			{
				// Check for argument quoting start
				if(c == '\'')
				{
					inQuotes = true;
					continue;
				}
				if(c == '"')
				{
					inDoubleQuotes = true;
					continue;
				}
			}

			if(c == ParameterPrefix)
			{
				// Save current parameter
				_arguments.TryAdd(parameter, sb.ToString().Trim());
				parameter = "";
				sb.Clear();
				i++;
				// Read the parameter (up until a space, " or ' character)
				while(i < args.Length && !(parameterPostfixes.Contains(args[i])))
				{
					parameter += args[i];
					i++;
				}
				continue;
			}

			sb.Append(c);
		}

		if(!string.IsNullOrEmpty(parameter))
		{
			_arguments.TryAdd(parameter, sb.ToString().Trim());
		}
		else if(sb.Length > 0)
		{
			_arguments.TryAdd("", sb.ToString().Trim());
		}
	}
}
using System.Diagnostics.CodeAnalysis;

namespace Nekai.Common;

public class ArgumentsReader {
	internal const char DEFAULT_PARAMETER_PREFIX = '-';
	protected Dictionary<string, string> _arguments = new(StringComparer.OrdinalIgnoreCase);

	public string ParameterlessArgument => _arguments[""];
	public char ParameterPrefix { get; protected set; }
	public string? this[string parameter] => _arguments.GetValueOrDefault(parameter);
	public string? this[params string[] parameters] => this[parameters.AsEnumerable()];
	public string? this[IEnumerable<string> parameters] => ReadAny(parameters);


	public ArgumentsReader(IEnumerable<string> args, char parameterPrefix = DEFAULT_PARAMETER_PREFIX)
		: this(string.Join(' ', args), parameterPrefix) { }

	public ArgumentsReader(string args, char parameterPrefix = DEFAULT_PARAMETER_PREFIX) {
		ParameterPrefix = parameterPrefix;
		Parse(args);
	}


	public bool TryRead(string parameter, [NotNullWhen(true)] out string? value) {
		value = this[parameter];
		return value is not null;
	}

	public string? Read(string? parameter) {
		return _arguments.GetValueOrDefault(parameter ?? "");
	}

	public string Read(string? parameter, string defaultValue) {
		parameter ??= "";
		return _arguments.ContainsKey(parameter)
			? _arguments[parameter]
			: defaultValue;
	}

	public string? ReadAny(params string?[] parameters)
		=> ReadAny(parameters.AsEnumerable());

	public string? ReadAny(IEnumerable<string?> parameters, string? defaultValue = null) {
		foreach(var parameter in parameters) {
			if(TryRead(parameter ?? "", out string? value))
				return value;
		}

		return defaultValue;
	}

	public bool TryReadAny(IEnumerable<string?> parameters, [NotNullWhen(true)] out string? value) {
		value = ReadAny(parameters);
		return value is not null;
	}


	protected void Parse(string args) {
		char[] parameterPostfixes = new[] { '\'', '"', ' ' };

		bool inQuotes = false;
		bool inDoubleQuotes = false;
		bool preceedingBackslash = false;
		string parameter = "";
		char c;
		StringBuilder sb = new();
		_arguments.Clear();

		for(int i = 0; i < args.Length; i++) {
			c = args[i];

			// Check for quotes end
			if(!preceedingBackslash) {
				if(inQuotes && c == '\'') {
					inQuotes = false;
					continue;
				}
				if(inDoubleQuotes && c == '"') {
					inDoubleQuotes = false;
					continue;
				}
			}

			if(inQuotes || inDoubleQuotes) {
				if(preceedingBackslash) {
					// Escape character
					sb.Append(c);
					preceedingBackslash = false;
					continue;
				}
				if(c == '\\') {
					// Escape the next character
					preceedingBackslash = true;
					continue;
				}
			} else if(sb.ToString().Trim().Length == 0) {
				// Check for argument quoting start
				if(c == '\'') {
					inQuotes = true;
					continue;
				}
				if(c == '"') {
					inDoubleQuotes = true;
					continue;
				}
			}

			if(c == ParameterPrefix) {
				// Save current parameter
				_arguments.TryAdd(parameter, sb.ToString().Trim());
				parameter = "";
				sb.Clear();
				i++;
				// Read the parameter (up until a space, " or ' character)
				while(i < args.Length && !(parameterPostfixes.Contains(args[i]))) {
					parameter += args[i];
					i++;
				}
				continue;
			}

			sb.Append(c);
		}

		if(!string.IsNullOrEmpty(parameter)) {
			_arguments.TryAdd(parameter, sb.ToString().Trim());
		} else if(sb.Length > 0) {
			_arguments.TryAdd("", sb.ToString().Trim());
		}
	}


}

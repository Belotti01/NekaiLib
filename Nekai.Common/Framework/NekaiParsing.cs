using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Nekai.Common;

public static class NekaiParsing
{
	[Pure]
	public static T Parse<T>(string value, IFormatProvider? formatProvider)
		where T : IParsable<T>
	{
		return T.Parse(value, formatProvider);
	}

	[Pure]
	public static bool TryParse<T>(string value, IFormatProvider? formatProvider, [NotNullWhen(true)] out T? parsedValue)
		where T : IParsable<T>
	{
		return T.TryParse(value, formatProvider, out parsedValue);
	}

	[Pure]
	public static bool CanParse<T>()
		=> CanParse(typeof(T));

	[Pure]
	public static bool CanParse(Type type)
	{
		return type.IsEnum
			|| type == typeof(string)
			|| _TryGetTryParseMethod(type, out _);
	}

	[Pure]
	public static T? Parse<T>(string value)
		=> (T?)Parse(value, typeof(T));

	[Pure]
	public static object? Parse(string value, Type type)
	{
		if(type == typeof(string))
			return value;

		if(type.IsEnum)
			return Enum.Parse(type, value);

		if(!_TryGetTryParseMethod(type, out MethodInfo? method))
			throw new InvalidTypeException(type, $"Parsing of string to type '{type.Name}' is not supported.");
		
		object?[] args = [ value, null ];
		bool parsed = (bool)method.Invoke(null, args)!;
		if(!parsed)
			throw new FormatException($"Value could not be parsed to type {type.Name}.");
		return args[1];
	}

	[Pure]
	public static bool TryParse<T>(string value, out T? parsedValue)
	{
		if(TryParse(value, typeof(T), out object? parsedObject))
		{
			Debug.Assert(parsedObject is not null, $"{typeof(T).Name}.TryParse() method returned true but the parsed value could not be retrieved.");
			parsedValue = (T?)parsedObject;
			return true;
		}
		parsedValue = default;
		return false;
	}

	[Pure]
	public static bool TryParse(string value, Type type, out object? parsedValue)
	{
		if(type == typeof(string))
		{
			parsedValue = value;
			return true;
		}

		parsedValue = null;
		if(type.IsEnum)
			return Enum.TryParse(type, value, out parsedValue);

		if(!_TryGetTryParseMethod(type, out MethodInfo? method))
			return false;   // No Parsing method found

		// args: { valueToParse, resultValue }
		object?[] args = [ value, null ];
		// TryParse methods don't necessarily guarantee that Exceptions won't be thrown. Just in case, wrap the call in a try-catch block.
		try
		{
			// Invoke the retrieved <type>.TryParse(string, out <type>) method
			bool parsed = (bool)method.Invoke(null, args)!;
			if(!parsed)
				return false;
			parsedValue = args[1];
			Debug.Assert(parsedValue is not null, "TryParse method returned true, but out parameter (the parsed value) has no value assigned.");
			return true;
		}
		catch
		{
			// TryParse method threw an Exception
			return false;
		}
	}

	[Pure]
	private static bool _TryGetTryParseMethod(Type type, [NotNullWhen(true)] out MethodInfo? tryParseMethod)
	{
		// Type parameter TSelf in IParsable<TSelf> does not matter - all is needed is the name of the function.
		// Note: The OUT parameter (aka the parsed value) is handled the same as REF parameters by Reflection methods.
		tryParseMethod = type.GetMethod(nameof(IParsable<int>.TryParse), BindingFlags.Static | BindingFlags.Public, [ typeof(string), type.MakeByRefType() ]);
		return tryParseMethod is not null;
	}
}
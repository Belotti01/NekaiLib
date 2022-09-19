using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Nekai.Common;

public static class TypeConversion {
    private static readonly Dictionary<Type, ICustomConverter> _customConverters;
    public static IEnumerable<Type> RegisteredConverters 
        => _customConverters.Keys;

    static TypeConversion() {
        _customConverters = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(x => x.GetInterface(nameof(ICustomConverter)) is not null)
            .Select(x => x.GetConstructor(Array.Empty<Type>())!.Invoke(null))
            .Cast<ICustomConverter>()
			.DistinctBy(x => x.Type)
            .ToDictionary(x => x.Type);
    }


    public static void SetConverter(ICustomConverter converter) {
        if(!_customConverters.TryAdd(converter.Type, converter)) {
            _customConverters[converter.Type] = converter;
        }
    }


    public static bool TryConvertTo<TOut>(this object value, [NotNullWhen(true)] out TOut? convertedValue) {
        convertedValue = value.ConvertTo<TOut>();
        return value is not null;
    }

    public static bool TryConvertTo(this object value, Type targetType, [NotNullWhen(true)] out object? convertedValue) {
        convertedValue = value.ConvertTo(targetType);
        return value is not null;
    }



    public static TOut? ConvertTo<TOut>(this object value)
        => value is null ? default : (TOut?)value.ConvertTo(typeof(TOut));

    public static object? ConvertTo(this object value, Type targetType) {
        if(targetType == value.GetType())
            return value;

        string stringValue = _ConvertToString(value);
        if(targetType == typeof(string))
            return stringValue;

        object? result = null;

        if(_customConverters.TryGetValue(targetType, out var converter)) {
            result = converter.ConvertFromString(stringValue);
        }
        result ??= _UseDefaultValueConversion(stringValue, targetType);
        
        return result;
    }


    private static string _ConvertToString<T>(T value) {
        if(value is string str)
            return str;

        Type sourceType = typeof(T);
        string? stringValue = null;

        if(_customConverters.ContainsKey(sourceType)) {
            stringValue = _customConverters[sourceType].ConvertToString(value);
        }
        stringValue ??= _UseDefaultStringConversion(value);

        return stringValue;
    }


	private static string _UseDefaultStringConversion<T>(T value) {
		return value?.ToString() ?? "";
	}

	private static object? _UseDefaultValueConversion(string value, Type targetType) {
        var interfaces = targetType.GetInterfaces();
        object? result = null;
		
        foreach(var implementation in interfaces) {
            if(implementation.IsGenericType) {
                if(implementation.GetGenericTypeDefinition() == typeof(IParsable<>)) {
                    var method = targetType.GetMethod("Parse", BindingFlags.Static | BindingFlags.Public, new[] { typeof(string) });
					result = method?.Invoke(null, new[] { value });
                }
            }
			
            if(result is not null)
                return result;
        }
        return result;
	}
}

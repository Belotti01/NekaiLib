using System.Numerics;

namespace Nekai.Common;

public static class NumberOperationExtensions
{
	public static T AddWithoutOverflow<T>(this T value, T other)
		where T : IMinMaxValue<T>, ISubtractionOperators<T, T, T>, INumber<T>, INumberBase<T>
	{
		if(T.MaxValue - other > value)
			return value + other;
		return T.MaxValue;
	}

    public static string ToScientificNotation(this double value, int decimals = 2)
        => value.ToString($"E{decimals}");
    
	public static string ToScientificNotation(this float value, int decimals = 2)
        => value.ToString($"E{decimals}");

    public static string ToScientificNotation(this decimal value, int decimals = 2)
        => value.ToString($"E{decimals}");

    public static string ToScientificNotation(this int value, int decimals = 2)
        => value.ToString($"E{decimals}");

    public static string ToScientificNotation(this long value, int decimals = 2)
        => value.ToString($"E{decimals}");

    public static string ToScientificNotation(this uint value, int decimals = 2)
        => value.ToString($"E{decimals}");

    public static string ToScientificNotation(this ulong value, int decimals = 2)
        => value.ToString($"E{decimals}");
}
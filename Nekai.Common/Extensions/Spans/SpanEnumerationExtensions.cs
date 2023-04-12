using System.Runtime.CompilerServices;

namespace Nekai.Common;

public static class SpanEnumerationExtensions
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool Any<T>(this ReadOnlySpan<T> span, Func<T, bool> predicate)
	{
		for(int i = span.Length - 1; i >= 0; --i)
		{
			if(predicate(span[i]))
				return true;
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool All<T>(this ReadOnlySpan<T> span, Func<T, bool> predicate)
	{
		for(int i = span.Length - 1; i >= 0; --i)
		{
			if(!predicate(span[i]))
				return false;
		}
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool Contains<T>(this ReadOnlySpan<T> span, T value)
		where T : struct
	{
		for(int i = span.Length - 1; i >= 0; --i)
		{
			if(span[i].Equals(value))
				return true;
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool ContainsAny<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> values)
		where T : struct
	{
		for(int i = values.Length - 1; i >= 0; --i)
		{
			if(span.Contains(values[i]))
				return true;
		}
		return false;
	}
}
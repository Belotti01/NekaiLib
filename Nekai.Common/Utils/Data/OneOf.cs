using System;
using System.Diagnostics.CodeAnalysis;

namespace Nekai.Common;

/// <summary>
/// A type that contains one of two values of different types.
/// </summary>
/// <typeparam name="T"> The type of the <see cref="Value"/> property. </typeparam>
/// <typeparam name="T2"> The type of the <see cref="AltValue"/> property. </typeparam>
/// <remarks> Use sparingly with value types to avoid boxing. When possible, prefer using <see cref="Result"/> over this. </remarks>
public class OneOf<T, T2>
{
	/// <summary> Creates a new <see cref="OneOf{T, T2}"/> with the specified value. </summary>
	/// <param name="value"> The contained value. </param>
	public static implicit operator OneOf<T, T2>(T value) => new(value);
	/// <summary> Creates a new <see cref="OneOf{T, T2}"/> with the specified value. </summary>
	/// <param name="value"> The contained value. </param>
	public static implicit operator OneOf<T, T2>(T2 value) => new(value);
	
	/// <summary> Whether the <see cref="OneOf{T, T2}"/> contains a value of type <typeparamref name="T"/>. </summary>
	[MemberNotNullWhen(true, nameof(Value)), MemberNotNullWhen(false, nameof(AltValue))]
	public bool IsValue { get; }
	/// <summary> Whether the <see cref="OneOf{T, T2}"/> contains a value of type <typeparamref name="T2"/>. </summary>
	[MemberNotNullWhen(true, nameof(AltValue)), MemberNotNullWhen(false, nameof(Value))]
	public bool IsAltValue => !IsValue;

	/// <summary> The contained value if <see cref="IsValue"/> is true, otherwise <see langword="null"/>. </summary>
	public T? Value { get; }
	/// <summary> The contained value if <see cref="IsAltValue"/> is true, otherwise <see langword="null"/>. </summary>
	public T2? AltValue { get; }


	private OneOf(T value)
	{
		Value = value;
		IsValue = true;
	}

	private OneOf(T2 value)
	{
		AltValue = value;
		IsValue = false;
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Nekai.Common;

public static class NumberToByteArrayConversionExtensions
{
    /// <inheritdoc cref="BitConverter.GetBytes(int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[] ToByteArray(this int value)
        => BitConverter.GetBytes(value);

    /// <inheritdoc cref="BitConverter.GetBytes(uint)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[] ToByteArray(this uint value)
        => BitConverter.GetBytes(value);

    /// <inheritdoc cref="BitConverter.GetBytes(short)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[] ToByteArray(this short value)
        => BitConverter.GetBytes(value);

    /// <inheritdoc cref="BitConverter.GetBytes(ushort)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[] ToByteArray(this ushort value)
        => BitConverter.GetBytes(value);

    /// <inheritdoc cref="BitConverter.GetBytes(long)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[] ToByteArray(this long value)
        => BitConverter.GetBytes(value);

    /// <inheritdoc cref="BitConverter.GetBytes(ulong)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[] ToByteArray(this ulong value)
        => BitConverter.GetBytes(value);

    /// <inheritdoc cref="BitConverter.GetBytes(float)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[] ToByteArray(this float value)
        => BitConverter.GetBytes(value);

    /// <inheritdoc cref="BitConverter.GetBytes(double)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[] ToByteArray(this double value)
        => BitConverter.GetBytes(value);

    /// <inheritdoc cref="BitConverter.GetBytes(Half)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[] ToByteArray(this Half value)
        => BitConverter.GetBytes(value);

    /// <inheritdoc cref="BitConverter.GetBytes(bool)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[] ToByteArray(this bool value)
        => BitConverter.GetBytes(value);

    /// <inheritdoc cref="BitConverter.GetBytes(char)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[] ToByteArray(this char value)
        => BitConverter.GetBytes(value);

    /// <summary> Return the given <paramref name="value"/> wrapped in an array of bytes. </summary>
    /// <returns> An array of bytes of length 1. </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[] ToByteArray(this byte value)
        => new[] { value };
    
    /// <summary>
    /// Convert the specified numeric <paramref name="value"/> to an array of <see langword="byte"/>.
    /// </summary>
    /// <typeparam name="T"> The type of the <paramref name="value"/> to convert. </typeparam>
    /// <param name="value"> The value to convert. </param>
    /// <returns> An array of <see langword="byte"/> containing the converted <paramref name="value"/>. </returns>
    /// <exception cref="InvalidTypeException"> The conversion for the specified type <typeparamref name="T"/> is not implemented. </exception>
    /// <remarks> This generic implementation should only be used when the concrete type of <paramref name="value"/> is unknown. </remarks>
    public static byte[] ToByteArray<T>(this T value) where T : INumber<T>
    {
        // Added to support generic numeric types.
        return value switch
        {
            byte b => new byte[] { b },
            ushort us => us.ToByteArray(),
            uint ui => ui.ToByteArray(),
            ulong ul => ul.ToByteArray(),
            sbyte sb => new byte[] { (byte)sb },
            short s => s.ToByteArray(),
            int i => i.ToByteArray(),
            long l => l.ToByteArray(),
            float f => f.ToByteArray(),
            double d => d.ToByteArray(),
            Half h => h.ToByteArray(),
            bool b => b.ToByteArray(),
            char c => c.ToByteArray(),
            Complex c => new byte[][] { c.Real.ToByteArray(), c.Imaginary.ToByteArray() }.SelectMany(x => x).ToArray(),
            _ => throw new InvalidTypeException(typeof(T), $"The type {typeof(T).Name} is not supported for byte array conversion.")
        };
    }
}

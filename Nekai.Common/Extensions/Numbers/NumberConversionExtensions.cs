using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Nekai.Common;

public static class NumberConversionExtensions
{
	/// <summary>
	/// Convert the specified numeric <paramref name="value"/> to an array of <see langword="byte"/>.
	/// </summary>
	/// <typeparam name="T"> The type of the <paramref name="value"/> to convert. </typeparam>
	/// <param name="value"> The value to convert. </param>
	/// <returns> An array of <see langword="byte"/> containing the converted <paramref name="value"/>. </returns>
	/// <exception cref="ArgumentException"> The conversion for the specified type <typeparamref name="T"/> is not implemented. </exception>
	public static byte[] ToByteArray<T>(this T value) where T : INumber<T>
	{
		return value switch
		{
			byte b => new byte[] { b },
			ushort us => BitConverter.GetBytes(us),
			uint ui => BitConverter.GetBytes(ui),
			ulong ul => BitConverter.GetBytes(ul),
			sbyte sb => new byte[] { (byte)sb },
			short s => BitConverter.GetBytes(s),
			int i => BitConverter.GetBytes(i),
			long l => BitConverter.GetBytes(l),
			float f => BitConverter.GetBytes(f),
			double d => BitConverter.GetBytes(d),
			Half h => BitConverter.GetBytes(h),
			bool b => BitConverter.GetBytes(b),
			char c => BitConverter.GetBytes(c),
			Complex c => new byte[][] { c.Real.ToByteArray(), c.Imaginary.ToByteArray() }.SelectMany(x => x).ToArray(),
			_ => throw new InvalidTypeException(typeof(T), $"The type {typeof(T).Name} is not supported for byte array conversion.")
		};
	}
}

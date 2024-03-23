using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Nekai.Common;

public static class NekaiMemory
{
	private const uint _MAX_32BIT_PROCESS_STACKALLOC_SIZE = 1024;

	/// <summary>
	/// The maximum memory size (NOT array length) of any stackalloc-based
	/// <see cref="Span{T}"/> creation.
	/// </summary>
	private static readonly uint _maxStackAllocSize;


	static NekaiMemory()
	{
		// 1MB is allocated to 32bit processes, and 4MB to 64bit processes
		_maxStackAllocSize = _MAX_32BIT_PROCESS_STACKALLOC_SIZE *
			(Environment.Is64BitProcess
				? 4u
				: 1u);
	}

#pragma warning disable CA1857 // A constant is expected for the parameter
    /// <summary>
    /// Check if a resulting array of length <paramref name="length"/> can be safely allocated onto the stack.
    /// </summary>
    /// <param name="length"> The length of the <see cref="Span{T}"/> to generate. </param>
    /// <remarks> If the type is known, use the <see cref="IsStackallocSafe(int, int)"/> overload instead.</remarks>
    public static bool IsStackallocSafe<T>(int length) where T : struct
        => IsStackallocSafe(length, Unsafe.SizeOf<T>());
#pragma warning restore CA1857 // A constant is expected for the parameter

    /// <summary>
    /// Check if a resulting array of length <paramref name="length"/> can be safely allocated onto the stack.
    /// </summary>
    /// <param name="length"> The length of the <see cref="Span{T}"/> to generate. </param>
    /// <param name="typeSize"> Use <c>sizeof(T)</c> on the type of the array elements. </param>
    public static bool IsStackallocSafe(int length, [ConstantExpected(Min = 1)] int typeSize)
	{
		// To avoid overflow errors, store the result in a bigger container type.
		ulong bitLength = (ulong)length * (ulong)typeSize;
		return bitLength <= _maxStackAllocSize;
	}
}
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Nekai.Common;

/// <summary>
/// Auto-generated GUID wrapper. Useful to implicitly automate the GUID generation during initialization of ID-bounded objects.
/// </summary>
public readonly struct AutoGuid : IParsable<AutoGuid>, ISpanParsable<AutoGuid>
{
	/// <summary> Cast the <paramref name="id"/> into its contained <see cref="Guid"/>. </summary>
	/// <param name="id"> The <see cref="AutoGuid"/> to cast. </param>
	public static implicit operator Guid(AutoGuid id) => id.Value;
	/// <summary> Wrap the <paramref name="id"/> into an <see cref="AutoGuid"/>. </summary>
	/// <param name="id"> The <see cref="Guid"/> to wrap. </param>
	public static explicit operator AutoGuid(Guid id) => new(id);

	private static readonly UniqueValueGenerator<Guid> _generator = new(Guid.NewGuid);

	/// <summary>
	/// The generated <see cref="Guid"/> value.
	/// </summary>
	public Guid Value { get; }

	/// <summary>
	/// Create a new instance of <see cref="AutoGuid"/> with a new random GUID value.
	/// </summary>
	public AutoGuid()
		: this(_generator.Next())
	{
	}

	/// <summary>
	/// Create a new instance of <see cref="AutoGuid"/> wrapping the <paramref name="guid"/>.
	/// </summary>
	private AutoGuid(Guid guid)
	{
		Value = guid;
	}

	/// <summary>
	/// Parse <paramref name="s"/> into a GUID and wrap it into an <see cref="AutoGuid"/> instance.
	/// </summary>
	/// <param name="s"> The <see langword="string"/> to parse. </param>
	/// <param name="provider"> The format information of <paramref name="s"/>. </param>
	/// <returns> <paramref name="s"/> parsed into an <see cref="AutoGuid"/>. </returns>
	public static AutoGuid Parse(string s, IFormatProvider? provider)
		=> Parse(s.AsSpan(), provider);

	/// <summary>
	/// Tries to parse the string into a GUID, and convert it to an <see cref="AutoGuid"/> instance.
	/// </summary>
	/// <param name="s"> The <see langword="string"/> to parse. </param>
	/// <param name="provider"> Provider of the format information of <paramref name="s"/>. </param>
	/// <param name="result"> The parsed GUID wrapped into an <see cref="AutoGuid"/>, or <see langword="default"/> if <paramref name="s"/> 
	/// is not a valid GUID format. </param>
	/// <returns> <see langword="true"/> if <paramref name="s"/> was successfully parsed into <paramref name="result"/>; 
	/// <see langword="false"/> otherwise. </returns>
	public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out AutoGuid result)
	{
		bool success = Guid.TryParse(s, provider, out Guid guid);
		result = (AutoGuid)guid;
		return success;
	}

	/// <inheritdoc cref="Parse(string, IFormatProvider?)"/>
	public static AutoGuid Parse(ReadOnlySpan<char> s, IFormatProvider? provider) 
		=> (AutoGuid)Guid.Parse(s, provider);

	/// <inheritdoc cref="TryParse(ReadOnlySpan{char}, IFormatProvider?, out AutoGuid)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out AutoGuid result)
		=> TryParse(s is null ? ReadOnlySpan<char>.Empty : s.AsSpan(), provider, out result);
}
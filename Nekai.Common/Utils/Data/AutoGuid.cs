using System.Diagnostics.CodeAnalysis;

namespace Nekai.Common;

/// <summary>
/// Self-assigning GUID.
/// </summary>
public readonly struct AutoGuid : IParsable<AutoGuid>
{
	public static implicit operator Guid(AutoGuid id) => id.Value;
	public static explicit operator AutoGuid(Guid id) => new(id);

	private static readonly UniqueValueGenerator<Guid> _generator = new(Guid.NewGuid);

	/// <summary>
	/// The generated GUID value.
	/// </summary>
	public Guid Value { get; }

	public AutoGuid()
		: this(_generator.Next())
	{
	}

	private AutoGuid(Guid guid)
	{
		Value = guid;
	}

	public static AutoGuid Parse(string s, IFormatProvider? provider)
	{
		return (AutoGuid)Guid.Parse(s, provider);
	}

	public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out AutoGuid result)
	{
		bool success = Guid.TryParse(s, provider, out Guid guid);
		result = (AutoGuid)guid;
		return success;
	}
}
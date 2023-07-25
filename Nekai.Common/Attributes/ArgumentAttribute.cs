using System.Diagnostics.CodeAnalysis;

namespace Nekai.Common;

/// <summary>
/// Defines that a Field or Property can be associated to an argument-parameter pair.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public sealed class ArgumentAttribute : Attribute, IArgumentAttribute
{
	/// <inheritdoc cref="IArgumentAttribute.Attribute"/>
	public string Attribute { get; private set; }

	/// <inheritdoc cref="IArgumentAttribute.Aliases"/>
	public string[] Aliases { get; private set; }

	/// <summary>
	/// Define the name and aliases for this argument-parameter pair.
	/// </summary>
	/// <param name="attribute">The primary name of this argument.</param>
	/// <param name="aliases">Eventual aliases that could be found instead of the <paramref name="attribute"/>.</param>
	public ArgumentAttribute([ConstantExpected] string attribute, params string[] aliases)
	{
		Attribute = attribute;
		Aliases = aliases;
	}

	/// <summary>
	/// Retrieve an <see cref="IEnumerable{T}"/> referencing all possible names that can be attributed to this
	/// argument-parameter pair.
	/// </summary>
	public IEnumerable<string> GetNames()
		=> Aliases.Prepend(Attribute);
}
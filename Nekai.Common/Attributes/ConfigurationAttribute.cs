using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Nekai.Common;

/// <summary>
/// Define that a Field or Property can be associated with a configuration element.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public sealed class ConfigurationAttribute : Attribute, IConfigurationAttribute
{
	/// <inheritdoc cref="IConfigurationAttribute.Names"/>
	public string[] Names { get; private set; }

	/// <inheritdoc cref="IConfigurationAttribute.DefaultValue"/>
	public string DefaultValue { get; set; } = "";

	/// <inheritdoc cref="IConfigurationAttribute.Description"/>
	public string? Description { get; set; }

	/// <summary>
	/// Define the name and eventual aliases for this configuration element.
	/// </summary>
	/// <param name="name">The primary name of this configuration element.</param>
	/// <param name="aliases">Eventual aliases that could be found for this configuration element.</param>
	public ConfigurationAttribute(string name, params string[] aliases)
	{
		Names = aliases.Prepend(name).ToArray();
		Debug.Assert(Names.All(x => !string.IsNullOrWhiteSpace(x)), "ConfigurationAttribute name or alias set to null or an empty string.");
	}

	/// <summary>
	/// Set the name for this configuration element to the name of the member.
	/// </summary>
	public ConfigurationAttribute([CallerMemberName] string memberName = null!)
	{
		Names = new[] { memberName };
		Debug.Assert(memberName is not null, "ConfigurationAttribute name was auto-set to member with unretrievable name.");
	}
}
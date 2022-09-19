namespace Nekai.Common;

/// <summary>
/// Define that a Field or Property can be associated with a configuration element.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class ConfigurationAttribute : Attribute, IConfigurationAttribute {
	/// <inheritdoc cref="IConfigurationAttribute.Names"/>
	public string[] Names { get; protected set; }
	/// <inheritdoc cref="IConfigurationAttribute.DefaultValue"/>
	public string DefaultValue { get; set; } = "";
	/// <inheritdoc cref="IConfigurationAttribute.Description"/>
	public string? Description { get; set; }

	/// <summary>
	/// Define the name and eventual aliases for this configuration element.
	/// </summary>
	/// <param name="name">The primary name of this configuration element.</param>
	/// <param name="aliases">Eventual aliases that could be found for this configuration element.</param>
	public ConfigurationAttribute(string name, params string[] aliases) {
		Names = aliases.Prepend(name).ToArray();
	}
}

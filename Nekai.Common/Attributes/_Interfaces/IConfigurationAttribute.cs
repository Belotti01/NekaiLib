namespace Nekai.Common;

/// <summary>
/// Defines a container of data regarding a configuration element.
/// </summary>
public interface IConfigurationAttribute {
	/// <summary>
	/// The name and eventual aliases for this configuration element.
	/// </summary>
	string[] Names { get; }
	/// <summary>
	/// The default string value for this configuration element.
	/// </summary>
	string DefaultValue { get; }
	/// <summary>
	/// Summary describing the function of this configuration element.
	/// </summary>
	string? Description { get; }


}

namespace Nekai.Common;

/// <summary>
/// Defines a container of data regarding an argument-parameter pair.
/// </summary>
public interface IArgumentAttribute
{
	/// <summary>
	/// The attribute to search for inside the arguments string to extract the value for this Field or Property.
	/// </summary>
	public string Attribute { get; }

	/// <summary>
	/// Alternative argument names for this Field or Property.
	/// </summary>
	public string[] Aliases { get; }

	/// <summary>
	/// Get a collection containing the <see cref="Attribute"/> and all possible <see cref="Aliases"/>
	/// of this argument.
	/// </summary>
	IEnumerable<string> GetNames();
}
namespace Nekai.Common;

/// <summary>
/// Represents an <see cref="Exception"/> that is thrown when an implementation is expected but missing. Usually
/// derived from the use of <see cref="System.Reflection"/> methods.
/// </summary>
public class MissingImplementationException : Exception
{
	/// <summary>The type of implementation that was expected (Field, Property, Method, etc.).</summary>
	public ImplementationType? ImplementationType { get; protected set; }

	/// <summary>The name of the implementation that was expected.</summary>
	public string? ImplementationName { get; protected set; }

	/// <summary>The name of the class that was supposed to contain the implementation.</summary>
	public string? ClassName { get; protected set; }

	/// <inheritdoc cref="MissingImplementationException(string, string, ImplementationType, Exception?)"/>
	public MissingImplementationException(string message, Exception? innerException = null)
		: base(message, innerException) { }

	/// <inheritdoc cref="MissingImplementationException(string, string, ImplementationType, Exception?)"/>
	internal MissingImplementationException(string implementationName, ImplementationType implementationType, Exception? innerException = null)
		: base($"{implementationType} {implementationName} could not be found.", innerException)
	{
		ImplementationType = implementationType;
		ImplementationName = implementationName;
	}

	/// <summary>
	/// Generate an <see cref="Exception"/> that contains information regarding a missing Field, Property, Method, etc.
	/// </summary>
	/// <param name="className">The name of the class that was supposed to contain the implementation.</param>
	/// <param name="implementationName">The name of the implementation that was expected.</param>
	/// <param name="implementationType">The type of implementation that was expected.</param>
	/// <param name="innerException">Eventual previouslt caught <see cref="Exception"/>.</param>
	internal MissingImplementationException(string className, string implementationName, ImplementationType implementationType, Exception? innerException = null)
		: base($"{implementationType} {implementationName} could not be found in Type {className}.", innerException)
	{
		ClassName = className;
		ImplementationType = implementationType;
		ImplementationName = implementationName;
	}
}
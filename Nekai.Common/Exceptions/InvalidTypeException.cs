namespace Nekai.Common;

/// <summary>
/// Represents error caused by invalid or conflicting <see cref="System.Type"/>s of objects or values.
/// </summary>
public class InvalidTypeException : Exception {
	/// <summary>
	/// The <see cref="System.Type"/> that caused the error.
	/// </summary>
	public Type Type { get; private init; }

	/// <inheritdoc cref="InvalidTypeException(Type, string, Exception?)"/>
	public InvalidTypeException(Type type, Exception? innerException = null) : this(type, $"Type {type.Name} is not valid in this context.", innerException) {
	}

	/// <summary>
	/// Generate an <see cref="Exception"/> that stores the <see cref="System.Type"/> that caused the error.
	/// </summary>
	/// <param name="type">The <see cref="System.Type"/> that caused the error.</param>
	/// <param name="message">Message displayed upon throwing this <see cref="Exception"/>.</param>
	/// <param name="innerException">Eventual previously caught <see cref="Exception"/>.</param>
	public InvalidTypeException(Type type, string message, Exception? innerException = null) : base(message, innerException) {
		Type = type;
	}
}

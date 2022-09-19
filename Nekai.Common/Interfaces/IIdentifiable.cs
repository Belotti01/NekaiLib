namespace Nekai.Common;

/// <summary>
/// Define a class that can be uniquely identified by its <see cref="Id"/> value.
/// </summary>
/// <typeparam name="TId"> The type of the <see cref="Id"/> identifier property. </typeparam>
public interface IIdentifiable<TId> {
	/// <summary>
	/// The unique identifier for this instance of the class.
	/// </summary>
	TId Id { get; }
}

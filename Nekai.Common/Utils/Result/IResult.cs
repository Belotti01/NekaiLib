namespace Nekai.Common;

// Used internally just to keep some level of consistency between concrete Result types.

/// <summary>
/// Represents the result of an operation with no return value.
/// </summary>
internal interface _IResult
{
	/// <summary>
	/// Whether the operation completed successfully.
	/// </summary>
	bool IsSuccess { get; }
	/// <summary>
	/// Information regarding the completion of the operation.
	/// </summary>
	string? Message { get; }
}

/// <summary>
/// Represents the result of an operation with a return value of <typeparamref name="TResult"/>.
/// </summary>
/// <typeparam name="TResult"> The type of the boxed return value. </typeparam>
internal interface _IResult<TResult> : _IResult
{
	/// <summary>
	/// The return value of the operation.
	/// </summary>
	TResult? Value { get; }
}

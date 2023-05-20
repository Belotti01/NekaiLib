using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Nekai.Common;

// For code readability when using Result<TResult>, this makes heavy use of implicit casting to centralize the implementation
// inside the non-generic Result.

// Readonly structs avoid memory allocation when not necessary.
// Note: Casts cause issues with nullability checks, so stick with using the properties for now.

/// <summary>
/// Represents the result of an operation with no return value, supplying an error message upon failure.
/// </summary>
public readonly struct Result : _IResult
{
	[MemberNotNullWhen(false, nameof(Message))]
	public bool IsSuccess { get; }
	public string? Message { get; }

	private Result(bool isSuccess, string? errorMessage)
	{
		Debug.Assert(isSuccess || errorMessage is not null, $"A message is required when generating a failure-signaling {nameof(Result)}.");
		Debug.Assert(isSuccess || errorMessage!.Trim().Length > 3, $"The supplied error message seems to not be sufficiently meaningful.");
		IsSuccess = isSuccess;
		Message = errorMessage;
	}

	/// <summary>
	/// Create an instance of <see cref="Result"/> representing a successful operation.
	/// </summary>
	public static Result Success()
		=> new(true, null);

	/// <summary>
	/// Create an instance of <see cref="Result{TResult}"/> representing a successful operation that returned <paramref name="value"/>.
	/// </summary>
	/// <param name="value"> The value returned by the operation. </param>
	public static Result<TResult> Success<TResult>(TResult? value)
		=> Result<TResult>.Success(value);

	/// <summary>
	/// Create an instance of <see cref="Result"/> representing a failed operation.
	/// </summary>
	/// <param name="errorMessage"> Message providing information regarding the failure of the operation. </param>
	public static Result Failure(string errorMessage)
		=> new(false, errorMessage);

	/// <summary>
	/// Create an instance of <see cref="Result{TResult}"/> representing a failed operation.
	/// </summary>
	/// <param name="errorMessage"> Message providing information regarding the failure of the operation. </param>
	public static Result<TResult> Failure<TResult>(string errorMessage)
		=> Result<TResult>.Failure(errorMessage);

	/// <summary>
	/// Create a <see cref="Result"/> from a <see langword="bool"/> value representing whether the operation was successful,
	/// and set the <see cref="Message"/> to the provided <paramref name="errorMessage"/> if not.
	/// </summary>
	/// <param name="isSuccess"> Whether the operation completed successfully. </param>
	/// <param name="errorMessage"> The value of the <see cref="Message"/> to set when the operation failed. </param>
	public static Result FromResult(bool isSuccess, string errorMessage)
		=> new(isSuccess, isSuccess ? null : errorMessage);

	/// <summary>
	///	Return a string representation of the result.
	/// </summary>
	/// <returns> 
	///	The error message if available, or the string representation of <see cref="IsSuccess"/> if not.
	///	</returns>
	public override string ToString()
		=> Message
		?? IsSuccess.ToString();
}

/// <summary>
/// Represents the result of an operation (with return type <typeparamref name="TResult"/>) that provides an
/// error message on failure.
/// </summary>
/// <typeparam name="TResult"> The type of the value returned by the caller. </typeparam>
public readonly struct Result<TResult> : _IResult<TResult>
{
	// Do not implement implicit operator to TResult, as it would be ambiguous (between IsSuccess and Value) with bool results.

	// The only way to avoid specifying the type everytime when using Result.Failure to create a Result<TResult>.
	public static implicit operator Result<TResult>(Result result)
		=> new(result.IsSuccess, result.Message, default!);

	public static implicit operator Result(Result<TResult> result)
		=> Result.FromResult(result.IsSuccess, result.Message ?? "");

	[MemberNotNullWhen(true, nameof(Value)), MemberNotNullWhen(false, nameof(Message))]
	public bool IsSuccess { get; }
	public string? Message { get; }
	public TResult Value { get; }

	private Result(bool isSuccess, string? errorMessage, TResult value)
	{
		Debug.Assert(isSuccess || errorMessage is not null, $"A message is required when generating a failure-signaling {nameof(Result<TResult>)}.");
		Debug.Assert(isSuccess || errorMessage!.Trim().Length > 3, $"The supplied error message seems to not be sufficiently meaningful.");
		Debug.Assert(!isSuccess || value is not null, $"A non-null value is required when generating a success-signaling {nameof(Result<TResult>)}.");
		IsSuccess = isSuccess;
		Message = errorMessage;
		Value = value;
	}

	/// <inheritdoc cref="Result.Success{TResult}(TResult)"/>
	public static Result<TResult> Success(TResult? value)
		=> new(true, default, value!);

	/// <inheritdoc cref="Result.Failure{TResult}(string)"/>
	public static Result<TResult> Failure(string errorMessage)
		=> new(false, errorMessage, default!);

	/// <summary>
	///	Return a string representation of the result.
	/// </summary>
	/// <returns> 
	///	The <see cref="Message"/> if available, or the string representation of <see cref="Value"/> if not.
	///	If both the <see cref="Message"/> and <see cref="Value"/> happen to be <see langword="null"/>, 
	///	fallback to the <see cref="IsSuccess"/> value.
	///	</returns>
	public override string ToString()
	{
		if(Message is not null)
			return Message;

		if(Value is not null)
		{
			try
			{
				string? result = Value.ToString();
				if(result is not null)
					return result;
			}
			catch { }
		}

		return IsSuccess.ToString();    // Fallback
	}
}

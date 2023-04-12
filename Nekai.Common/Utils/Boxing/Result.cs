using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Microsoft.Diagnostics.Runtime.Utilities;

namespace Nekai.Common;

// Readonly structs avoid memory allocation when not necessary.
// Note: Casts cause issues with nullability checks, so stick with using the properties for now.
// TODO: When possible, implement compiler checks for nullability, as attributes such as [NotNullWhen(...)] do not work with non-bool return values,
// even with implicit Result-to-bool casting.

/// <summary>
/// Represents the result of an operation with no return value, supplying an error message upon failure.
/// </summary>
public readonly struct Result : _IResult
{
	[MemberNotNullWhen(false, nameof(Message))]
	public bool IsSuccess { get; }
	public string? Message { get; }
	
	private Result(bool isSuccess, string? message)
	{
		Debug.Assert(isSuccess || message is not null, $"A message is required when generating a failure-signaling {nameof(Result)}.");
		Debug.Assert(isSuccess || message!.Trim().Length > 3, $"The supplied error message seems to not be sufficiently meaningful.");
		IsSuccess = isSuccess;
		Message = message;
	}

	public static Result Success()
		=> new(true, null);

	// Basically just a short-hand version of Result<TResult>.Success(), since TResult can be deduced from the value parameter.
	public static Result<TResult> Success<TResult>(TResult value)
		=> Result<TResult>.Success(value);

	public static Result<TResult> SuccessInline<TResult>(TResult value)
		=> Result<TResult>.Success(value);

	public static Result Failure(string message)
		=> new(false, message);

	// For completeness.
	public static Result<TResult> Failure<TResult>(string message)
		=> Result<TResult>.Failure(message);

	public static Result FromValue(bool isSuccess, string? message = null)
		=> new(isSuccess, isSuccess ? null : message);

	public static Result FromResult<TResult>(Result<TResult> result)
		=> FromValue(result.IsSuccess, result.Message);

	/// <summary>
	///		Return a string representation of the result.
	/// </summary>
	/// <returns> 
	///		The error message if available, or the string representation of <see cref="IsSuccess"/> if not.
	///	</returns>
	public override string ToString()
		=> Message
		?? IsSuccess.ToString();
}

/// <summary>
/// Represents the result of an operation that returns an instance of <typeparamref name="TResult"/> when successful, or an
/// error message on failure.
/// </summary>
/// <typeparam name="TResult"> The type of the value returned by the caller. </typeparam>
public readonly struct Result<TResult> : _IResult<TResult>
{
	// Do not implement implicit operator from Result<TResult> to TResult, as it would be ambiguous with bool results.
	
	public static implicit operator Result<TResult>(Result result) 
		=> new(result.IsSuccess, result.Message, default);

	[MemberNotNullWhen(true, nameof(Value)), MemberNotNullWhen(false, nameof(Message))]
	public bool IsSuccess { get; }
	public string? Message { get; }
	public TResult? Value { get; }

	private Result(bool isSuccess, string? message, TResult? value)
	{
		Debug.Assert(isSuccess || message is not null, $"A message is required when generating a failure-signaling {nameof(Result<TResult>)}.");
		Debug.Assert(isSuccess || message!.Trim().Length > 3, $"The supplied error message seems to not be sufficiently meaningful.");
		Debug.Assert(!isSuccess || value is not null, $"A non-null value is required when generating a success-signaling {nameof(Result<TResult>)}.");
		IsSuccess = isSuccess;
		Message = message;
		Value = value;
	}

	public static Result<TResult> Success(TResult value)
		=> new(true, default, value);

	public static Result<TResult> Failure(string message)
		=> new(false, message, default);
	
	public static Result<TResult> FromValue(TResult value, bool isSuccess, string messageIfFailure)
		=> new(isSuccess, isSuccess ? null : messageIfFailure, value);

	public static Result<TResult> FromResult(Result<TResult> result, string? newMessage = null)
		=> new(result.IsSuccess, newMessage ?? result.Message, result.Value);

	/// <summary>
	///		Return a string representation of the result.
	/// </summary>
	/// <returns> 
	///		The <see cref="Message"/> if available, or the string representation of <see cref="Value"/> if not.
	///		If both the <see cref="Message"/> and <see cref="Value"/> happen to be <see langword="null"/>, 
	///		fallback to the <see cref="IsSuccess"/> value.
	///	</returns>
	public override string ToString() 
		=> Message 
		?? Value?.ToString()
		?? IsSuccess.ToString();	// Fallback
}

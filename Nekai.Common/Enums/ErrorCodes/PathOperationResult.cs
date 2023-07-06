using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nekai.Common;

[OperationResult]
public enum PathOperationResult
{
	/// <summary> Default value; the operation completed successfully. </summary>
	Success = 0,
	/// <summary> The provided path contains invalid characters. </summary>
	ContainsInvalidPathChars,
	/// <summary> The provided path is not valid. </summary>
	InvalidPath,
	/// <summary> The provided path is longer than allowed by the OS. </summary>
	PathTooLong,
	/// <summary> A null, empty, or whitespace path argument was provided. </summary>
	PathIsEmpty,
	/// <summary> The file or directory does not exist. </summary>
	DoesNotExist,
	/// <summary> The file or directory already exists. </summary>
	AlreadyExists,
	/// <summary> The file is read-only. </summary>
	IsReadOnly,
	/// <summary> The file is a system file. </summary>
	IsSystem,
	/// <summary> The contents of the file are not in the expected format. </summary>
	BadFormat,
	/// <summary> The operation is not allowed by the OS. </summary>
	NotAllowed,
	/// <summary> The read operation failed. </summary>
	FailedRead,
	/// <summary> An unknown error occurred. </summary>
	UnknownFailure,
}

public static class PathOperationResultExtensions
{

	public static bool IsSuccess(this PathOperationResult result)
		=> result == PathOperationResult.Success;

	public static string GetMessage(this PathOperationResult result)
		=> result switch
		{
			PathOperationResult.Success => "The operation completed successfully.",
			PathOperationResult.ContainsInvalidPathChars => "The provided path contains invalid characters.",
			PathOperationResult.InvalidPath => "The provided path is not valid.",
			PathOperationResult.PathTooLong => "The provided path is longer than allowed by the OS.",
			PathOperationResult.PathIsEmpty => "A null, empty, or whitespace path argument was provided.",
			PathOperationResult.DoesNotExist => "The file or directory does not exist.",
			PathOperationResult.AlreadyExists => "The file or directory already exists.",
			PathOperationResult.IsReadOnly => "The file is read-only.",
			PathOperationResult.IsSystem => "The file is a system file.",
			PathOperationResult.NotAllowed => "The operation is not allowed by the OS.",
			PathOperationResult.UnknownFailure => "An unknown error occurred.",
			_ => throw new NotImplementedException(),
		};
}

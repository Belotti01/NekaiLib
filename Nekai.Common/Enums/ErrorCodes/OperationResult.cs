using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Nekai.Common.DesignPatterns.Attributes;

namespace Nekai.Common;


/// <summary>
/// General-purpose operation results.
/// </summary>
/// <remarks>
/// Prefer using your own more specific OperationResult enum with the <see cref="OperationResultAttribute"/> attribute, 
/// while mapping values to the ones that are defined here when possible. This ensures easier handling of errors while
/// keeping scalability.
/// </remarks>
[OperationResult]
public enum OperationResult
{
	Success = 0,

	InvalidParameter,
	
	UnknownError
}

public static class OperationResultExtensions
{
	/// <summary>
	/// Whether the operation that generated this result has completed successfully.
	/// </summary>
	/// <param name="result"> The generated result. </param>
	/// <returns> <see langword="true"/> if the value of the <see cref="OperationResult"/> is 
	/// <see cref="OperationResult.Success"/>; <see langword="false"/> otherwise. </returns>
	public static bool IsSuccessful(this OperationResult result)
		=> result == OperationResult.Success;
}

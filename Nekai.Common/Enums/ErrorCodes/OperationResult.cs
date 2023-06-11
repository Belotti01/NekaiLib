using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Nekai.Common;

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

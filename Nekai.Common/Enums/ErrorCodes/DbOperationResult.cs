namespace Nekai.Common;

[OperationResult]
public enum DbOperationResult
{
	Success = 0,
	ConnectionLost,
	UnavailableSource,
	TimedOut,
	IsBusy,
	NotFound,
	InvalidParameter,
	QueryError,
	UnknownError
}

public static class DbOperationResultExtensions
{
	public static bool IsSuccessful(this DbOperationResult result) 
		=> result == DbOperationResult.Success;
}
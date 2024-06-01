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
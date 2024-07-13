namespace Nekai.Common;

[OperationResult]
public enum NetworkOperationResult
{
	Success = 0,
	NotConnected,
	NoInternet,
	InvalidTarget,
	TargetNotFound,
	DnsError,
	ProxyError,
	TimedOut,
	InvalidRequest,
	InvalidResponse,
	InvalidOperation,
	BadFormat,
	UnknownError
}

public static class NetworkOperationResultExtensions
{
	public static bool IsSuccessful(this NetworkOperationResult result)
		=> result == NetworkOperationResult.Success;
}
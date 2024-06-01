namespace Nekai.Common;

[OperationResult]
public enum NetworkOperationResult
{
	Success = 0,
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
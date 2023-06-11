using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
	UnknownError
}

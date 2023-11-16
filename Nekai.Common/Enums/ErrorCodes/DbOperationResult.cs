using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

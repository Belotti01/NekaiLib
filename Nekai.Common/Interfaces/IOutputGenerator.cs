using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Nekai.Common;

public interface IOutputGenerator<TSelf>
	where TSelf : IOutputGenerator<TSelf>
{
	TSelf Write(string? message, LogLevel level = LogLevel.Information);

	virtual TSelf Warning(string? message)
		=> Write(message, LogLevel.Warning);

	virtual TSelf Error(string? message)
		=> Write(message, LogLevel.Error);
	virtual TSelf Fatal(string? message)
		=> Write(message, LogLevel.Critical);

	virtual TSelf WriteDebug(string? message)
		=> CurrentApp.IS_DEBUG
			? Write(message, LogLevel.Debug)
			: (TSelf)this;
}

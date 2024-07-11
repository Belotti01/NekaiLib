using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nekai.Common;

/// <summary>
/// Exception type that writes the given message as a shared log before throwing.
/// </summary>
public class NekaiInternalException : Exception
{
	internal NekaiInternalException(string message) : base(message) 
	{
		NekaiLogs.Shared.Error(message);
	}

	internal NekaiInternalException(string message, string parameterName) : this(message + $" (parameter '{parameterName}')") { }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nekai.Common;

/// <summary>
/// A <see langword="string"/> to be used to indicate an error message.
/// </summary>
public class ErrorString
{
	public static implicit operator string(ErrorString error) => error.Error;
	public static implicit operator ErrorString(string error) => new(error);
	public static implicit operator ErrorString(Exception ex) => new(ex);

	public string Error { get; set; }

	public ErrorString(string error)
	{
		Error = error;
	}

	public ErrorString(Exception exception)
	{
		Error = exception.GetFullMessage();
	}
}

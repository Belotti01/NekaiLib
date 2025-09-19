namespace Nekai.Common;

public static class ExceptionExtensions
{
	/// <summary>
	/// Retrieve a concatenation of <see cref="Exception.Message"/>s for the exception and all its inner exceptions.
	/// </summary>
	public static string GetFullMessage(this Exception exception)
	{
		string result = exception.Message;

		while(exception.InnerException is {} currentException)
		{
			if(result.EndsWith('.'))
				result += ' ' + currentException.Message;
			else
				result += ". " + currentException.Message;
		}

		return result;
	}
}
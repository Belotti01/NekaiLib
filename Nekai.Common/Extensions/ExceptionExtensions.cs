namespace Nekai.Common;

public static class ExceptionExtensions
{
	/// <summary>
	/// Retrieve a concatenation of <see cref="Exception.Message"/>s for the exception and all its inner exceptions.
	/// </summary>
	public static string GetFullMessage(this Exception exception)
	{
		Exception? currentException;
		string result = exception.Message;

		while((currentException = exception.InnerException) is not null)
		{
			result += " - " + currentException.Message;
		}

		return result;
	}
}
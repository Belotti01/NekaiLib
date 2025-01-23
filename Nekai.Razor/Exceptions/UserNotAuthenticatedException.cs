namespace Nekai.Razor;

public class UserNotAuthenticatedException : Exception
{
	public UserNotAuthenticatedException()
		: base("The user is not authenticated.")
	{
		
	}

	public UserNotAuthenticatedException(string message)
		: base(message)
	{
		
	}
}
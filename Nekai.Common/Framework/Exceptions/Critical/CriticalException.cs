namespace Nekai.Common;

// Generation of CriticalExceptions should be limited to the Exceptor class, so that we can prepend additional
// behavior before exiting the application.
// Hence why the class is scoped to internal.
public static partial class Exceptor
{
	internal class _CriticalException : Exception
	{
		public AppExitCode ExitCode { get; }

		internal _CriticalException(AppExitCode exitCode, Exception? innerException = null)
			: this(exitCode, "", innerException)
		{
		}

		internal _CriticalException(AppExitCode exitCode, string message, Exception? innerException = null)
			: base(_ExpandMessage(message, exitCode), innerException)
		{
			ExitCode = exitCode;
		}

		private static string _ExpandMessage(ReadOnlySpan<char> message, AppExitCode exitCode)
		{
			message = "A critical error has occurred" + (message.IsEmpty
				? '.'
				: $": {message}.");

			return $"[{(int)exitCode}: {exitCode}] {message}";
		}
	}
}
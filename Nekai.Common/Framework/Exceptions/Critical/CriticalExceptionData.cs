namespace Nekai.Common;

// Required for removing access to throwing CriticalExceptions, while still keeping access to the contained data.
// Also includes an API for keeping additional information fed from event handlers.

/// <summary>
/// Information regarding an instance of <see cref="Exceptor._CriticalException"/>.
/// </summary>
public sealed class CriticalExceptionData
{
	/// <inheritdoc cref="Exception.Message"/>
	public string Message { get; }

	/// <inheritdoc cref="Exception.StackTrace"/>
	public string? StackTrace { get; }

	/// <inheritdoc cref="Exceptor._CriticalException.ExitCode"/>
	public AppExitCode ExitCode { get; }

	/// <inheritdoc cref="Exception.InnerException"/>
	public Exception? InnerException { get; }

	/// <summary> Whether the application should exit with exit code <see cref="ExitCode"/> after all handlers
	/// have been invoked. Defaults to <see langword="true"/>. </summary>
	public bool ExitApplication { get; set; } = true;

	private readonly List<string> _additionalDumpInformation = [];

	internal CriticalExceptionData(Exceptor._CriticalException ex)
	{
		Message = ex.Message;
		StackTrace = ex.StackTrace;
		ExitCode = ex.ExitCode;
		InnerException = ex.InnerException;
	}

	/// <summary>
	/// Feed additional information that will be exported along with the exception.
	/// </summary>
	public void AddDumpInformation(params string[] info)
		=> _additionalDumpInformation.AddRange(info);

	/// <summary>
	/// Format the information stored in this instance into a <see langword="string"/>.
	/// </summary>
	public override string ToString()
	{
		const int DIV_LENGTH = 10;
		const string DUMP_INFO_STRING = "ADDITIONAL DUMP INFORMATION:";
		const string STACK_TRACE_STRING = "STACK TRACE:";

		// Calculate the length of the string to avoid reallocating the string multiple times.
		int stringLength = Message.Length
			+ (
				_additionalDumpInformation.Count == 0
				? 0
				: DIV_LENGTH + DUMP_INFO_STRING.Length + _additionalDumpInformation.Sum(s => s.Length + 2)
			) + (
				string.IsNullOrWhiteSpace(StackTrace)
				? 0
				: DIV_LENGTH + STACK_TRACE_STRING.Length + StackTrace.Length + 2
			);

		StringBuilder sb = new(stringLength);
		// MESSAGE
		sb.AppendLine(Message);

		// ADDITIONAL DUMP INFORMATION
		if(_additionalDumpInformation.Any())
		{
			sb.Append('-', DIV_LENGTH).AppendLine(DUMP_INFO_STRING);
			foreach(string info in _additionalDumpInformation)
			{
				sb.AppendLine(info);
			}
		}

		// STACK TRACE
		if(!string.IsNullOrWhiteSpace(StackTrace))
		{
			sb.Append('-', DIV_LENGTH).AppendLine(STACK_TRACE_STRING);
			sb.AppendLine(StackTrace);
		}

		return sb.ToString();
	}
}
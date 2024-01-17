using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Serilog.Events;

namespace Nekai.Common;

// The class is split into multiple files: this contains the logic for CriticalExceptions.
// Called Exceptor instead of Throw/ThrowHelper to avoid collision with other external libraries.

/// <summary>
/// Helper class that provides methods to throw and log exceptions, but also safely handle critical errors that require
/// the program to exit.
/// </summary>
public static partial class Exceptor
{

	public delegate void CriticalExceptionHandler(CriticalExceptionData exception);
	/// <summary> Invoked upon throwing a <see cref="_CriticalException"/>, before ending the Application's execution. </summary>
	/// <remarks> Does not replace <see cref="CurrentApp.OnProcessExit"/> when throwing a critical exception; this will be fired first. </remarks>
	public static event CriticalExceptionHandler? OnCriticalException;


	[StackTraceHidden, DebuggerStepThrough]
	public static void ThrowCritical(AppExitCode exitCode, Exception? innerException = null)
		=> _ThrowCriticalException(new _CriticalException(exitCode, innerException));

	[StackTraceHidden, DebuggerStepThrough]
	public static void ThrowCritical(AppExitCode exitCode, string message, Exception? innerException = null)
		=> _ThrowCriticalException(new _CriticalException(exitCode, message, innerException));

	[Conditional("DEBUG"), DoesNotReturn, StackTraceHidden, DebuggerStepThrough]
	public static void ThrowIfDebug(string msg, Exception? innerException = null)
		=> _Throw(new Exception(msg, innerException));

	[Conditional("DEBUG"), DoesNotReturn, StackTraceHidden, DebuggerStepThrough]
	public static void ThrowAndLogIfDebug(string msg, Exception? internalException = null)
		=> ThrowAndLog(new Exception(msg, internalException), LogEventLevel.Debug);

	[Conditional("DEBUG"), DoesNotReturn, StackTraceHidden, DebuggerStepThrough]
	public static void ThrowAndLogIfDebug<TException>(TException ex)
		where TException : Exception
		=> ThrowAndLog(ex, LogEventLevel.Debug);

	[DoesNotReturn, StackTraceHidden, DebuggerStepThrough]
	public static void ThrowAndLogError<TException>(TException ex)
		where TException : Exception
	{
		NekaiLogs.Shared.Error(ex);
		throw ex;
	}

	[DoesNotReturn, StackTraceHidden, DebuggerStepThrough]
	public static void ThrowAndLogError(string? msg)
		=> ThrowAndLogError(new Exception(msg));

	[DoesNotReturn, StackTraceHidden, DebuggerStepThrough]
	public static void ThrowAndLogWarning(string msg)
		=> ThrowAndLog(new Exception(msg), LogEventLevel.Warning);

	[DoesNotReturn, StackTraceHidden, DebuggerStepThrough]
	public static void ThrowAndLog<TException>(TException ex)
		where TException : Exception
		=> ThrowAndLog(ex, LogEventLevel.Warning);

	[DoesNotReturn, StackTraceHidden, DebuggerStepThrough]
	private static void _Throw<TException>(TException ex)
		where TException : Exception
		=> throw ex;

	[DoesNotReturn, StackTraceHidden, DebuggerStepThrough]
	public static void ThrowAndLog<TException>(TException ex, LogEventLevel logType)
		where TException : Exception
	{
		NekaiLogs.Shared.Write(ex, logType);
		_Throw(ex);
	}


	[StackTraceHidden]
	private static void _ThrowCriticalException(_CriticalException ex)
	{
		// Assume that everything is now broken and you need to somehow:
		// 1) Launch any attached handlers
		// 2) Dump all the information we have on the error
		// 3) Safely exit the application

		CriticalExceptionData data = new(ex);
		// Exception handlers might pre-emptively halt execution.
		// To keep as much information as possible, while minimizing the risk of skipping the exportation process,
		// temporarily delegate the operation to when the process exits.
		EventHandler infoDumpDelegate = (_, _) => _ = _TryDumpCriticalExceptionInfo(data);
		CurrentApp.OnProcessExit += infoDumpDelegate;

		try
		{
			OnCriticalException?.Invoke(data);
		}
		catch(Exception exception)
		{
			data.AddDumpInformation("An operation delegated to handle critical exceptions errored.", exception.ToString());
		}
		// It's still safer to not wait until the process is dying to infodump, so remove the delegate if it can be done now...
		if(_TryDumpCriticalExceptionInfo(data))
		{
			CurrentApp.OnProcessExit -= infoDumpDelegate;
		}
		// ... otherwise let it retry once more on process exit.

		if(data.ExitApplication)
			Environment.Exit((int)data.ExitCode);
	}

	[StackTraceHidden]
	private static bool _TryDumpCriticalExceptionInfo(CriticalExceptionData data)
	{
		try
		{
			NekaiLogs.Shared.Fatal(data.ToString());
			return true;
		}
		catch
		{
			// Critical Exception might origin from the Logger methods, or they may have been affected somehow.
			try
			{
				// Improvise a dump file
				string rawPath = Path.Combine(Directory.GetCurrentDirectory(), $"CRITICAL_ERROR_DUMP-{DateTime.Now:yyyyMMdd_hhmmss}.txt");
				var path = PathString.Parse(rawPath);
				var creationResult = path.EnsureExistsAsFile();
				if(creationResult != PathOperationResult.Success)
					return false;

				File.WriteAllText(path, data.ToString());
				return true;
			}
			catch(Exception ex)
			{
				ThrowAndLogIfDebug(ex);
			}
		}
		return false;
	}
}
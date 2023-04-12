using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Serilog.Events;

namespace Nekai.Common;

// Called Exceptor instead of Throw/ThrowHelper to avoid collision with other libraries.
public static partial class Exceptor
{

	public delegate void CriticalExceptionHandler(CriticalExceptionData exception);
	/// <summary> Invoked upon throwing a <see cref="CriticalException"/>, before ending the Application's execution. </summary>
	public static event CriticalExceptionHandler? OnCriticalException;

	
	[StackTraceHidden]
	public static void ThrowCritical(AppExitCode exitCode, Exception? innerException = null)
		=> _ThrowCriticalException(new CriticalException(exitCode, innerException));
		
	[StackTraceHidden]
	public static void ThrowCritical(AppExitCode exitCode, string message, Exception? innerException = null)
		=> _ThrowCriticalException(new CriticalException(exitCode, message, innerException));

	[Conditional("DEBUG")]
	[DoesNotReturn]
	[StackTraceHidden]
	public static void ThrowIfDebug(string msg, bool writeLog = true)
	{
		if(writeLog)
			ThrowAndLog(new Exception(msg), LogEventLevel.Debug);
		_Throw(new Exception(msg));
	}

	[Conditional("DEBUG")]
	[DoesNotReturn]
	[StackTraceHidden]
	public static void ThrowIfDebug<TException>(TException ex, bool writeLog = true)
		where TException : Exception
		=> ThrowAndLog(ex, LogEventLevel.Debug);

	[DoesNotReturn]
	[StackTraceHidden]
	public static void ThrowAndLogError<TException>(TException ex)
		where TException : Exception
	{
		NekaiLogs.Shared.Error(ex);
		throw ex;
	}

	[DoesNotReturn]
	[StackTraceHidden]
	public static void ThrowAndLogError(string? msg)
		=> ThrowAndLogError(new Exception(msg));

	[DoesNotReturn]
	[StackTraceHidden]
	public static void ThrowAndLogWarning(string msg)
		=> ThrowAndLog(new Exception(msg), LogEventLevel.Warning);

	[DoesNotReturn]
	[StackTraceHidden]
	public static void ThrowAndLog<TException>(TException ex)
		where TException : Exception
		=> ThrowAndLog(ex, LogEventLevel.Warning);

	[DoesNotReturn]
	[StackTraceHidden]
	private static void _Throw<TException>(TException ex)
		where TException : Exception
		=> throw ex;

	[DoesNotReturn]
	[StackTraceHidden]
	public static void ThrowAndLog<TException>(TException ex, LogEventLevel logType)
		where TException : Exception
	{
		NekaiLogs.Shared.Write(ex, logType);
		_Throw(ex);
	}


	[StackTraceHidden]
	private static void _ThrowCriticalException(CriticalException ex)
	{
		// Assume that everything is now broken and you need to somehow:
		// 1) Launch any attached handlers
		// 2) Dump all the information we have
		// 3) Exit the application

		CriticalExceptionData data = new(ex);
		// Exception handlers might pre-emptively halt execution.
		// To keep as much information as possible, while minimizing the risk of skipping the exportation process,
		// temporarily delegate the operation to when the process exits
		EventHandler infoDumpDelegate = (_, _) => _ = _TryDumpCriticalExceptionInfo(data);
		CurrentApp.OnProcessExit += infoDumpDelegate;

		try
		{
			OnCriticalException?.Invoke(data);
		}
		catch(Exception exception)
		{
			data.AddDumpInformation("Handler delegated to critical exception errored.", exception.ToString());
		}
		// It's still safer to not wait until the process is dying to infodump, so remove the delegate if
		// it can be done now...
		if(_TryDumpCriticalExceptionInfo(data))
		{
			CurrentApp.OnProcessExit -= infoDumpDelegate;
		}
		// ... otherwise let it retry once more on process exit.
		
		if(data.ExitApplication)    
			Environment.Exit((int)data.ExitCode);
	}

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
				string dir = Directory.GetCurrentDirectory();
				string dumpFile = Path.Combine(dir, $"CRITICAL_ERROR_DUMP-{DateTime.Now:yyyyMMdd_hhmmss}.txt");
				Result creationResult = NekaiFile.TryCreateOrOverwrite(dumpFile);
				if(!(creationResult.IsSuccess))
					return false;

				File.WriteAllText(dumpFile, data.ToString());
				return true;
			}
			catch { }
		}
		return false;
	}
}
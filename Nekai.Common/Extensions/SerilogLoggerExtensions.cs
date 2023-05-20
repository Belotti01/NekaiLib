using Serilog;
using Serilog.Events;

namespace Nekai.Common;

public static class SerilogLoggerExtensions
{
	public static void ThrowWarningIfDebug(this ILogger logger, string? message, params object?[] propertyValues)
		=> _ThrowIfDebug(logger, message, LogEventLevel.Warning, propertyValues);

	public static void ThrowErrorIfDebug(this ILogger logger, string? message, params object?[] propertyValues)
		=> _ThrowIfDebug(logger, message, LogEventLevel.Error, propertyValues);

	public static void Warning(this ILogger logger, Exception ex)
	{
		logger.Warning(_FormatForLogging(ex));
	}

	public static void Error(this ILogger logger, Exception ex)
	{
		logger.Error(_FormatForLogging(ex));
	}

	public static void Fatal(this ILogger logger, Exception ex)
	{
		logger.Fatal(_FormatForLogging(ex));
	}

	public static void Write(this ILogger logger, Exception ex, LogEventLevel level)
	{
		logger.Write(level, _FormatForLogging(ex));
	}

	private static string _FormatForLogging(Exception ex)
	{
		if(CurrentApp.HasDebugger)
			return ex.ToString();
		return ex.Message;
	}

	private static void _ThrowIfDebug(ILogger logger, string? message, LogEventLevel level, params object?[] propertyValues)
	{
		if(message is not null)
		{
			logger.Write(level, message, propertyValues);
			Exceptor.ThrowIfDebug(string.Format(message, propertyValues));
		}
		else
		{
			Exceptor.ThrowIfDebug("An unspecified DEBUG error occurred.");
		}
	}
}

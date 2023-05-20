using Serilog.Events;
using Serilog.Formatting;
using Serilog.Formatting.Compact;
using Serilog.Formatting.Json;

namespace Nekai.Common;

public class NekaiLoggerConfiguration
{
	public string OutputFilePathTemplate { get; protected set; } = Path.Combine("Logs", "Logs.json");
	public bool LogToConsole { get; set; } = false;
	public bool LogToFile { get; set; } = false;
	public bool UseFormatter { get; protected set; } = false;
	public string StringFormat { get; protected set; } = "{Timestamp:HH:mm:ss} [{Level:u4}] {Message:lj}{NewLine}{Exception}";
	public ITextFormatter Formatter { get; protected set; } = new CompactJsonFormatter();
	public LogEventLevel MinimumConsoleLogLevel { get; set; } = LogEventLevel.Information;
	public LogEventLevel MinimumFileLogLevel { get; set; } = LogEventLevel.Information;


	public NekaiLoggerConfiguration WithConsoleOutput()
	{
		LogToConsole = true;
		return this;
	}

	public NekaiLoggerConfiguration WithFileOutput(string? logFileTemplate = null, string? filePath = null, LogEventLevel? minimumLogLevel = null)
	{
		LogToFile = true;
		if(filePath is not null)
		{
			OutputFilePathTemplate = filePath;
		}
		if(logFileTemplate is not null)
		{
			UseFormatter = false;
			StringFormat = logFileTemplate;
		}
		if(minimumLogLevel is not null)
		{
			MinimumFileLogLevel = minimumLogLevel.Value;
		}
		return this;
	}

	public NekaiLoggerConfiguration WithFileOutput(ITextFormatter? formatter = null, string? filePath = null, LogEventLevel? minimumLogLevel = null)
	{
		LogToFile = true;
		if(filePath is not null)
		{
			OutputFilePathTemplate = filePath;
		}
		if(formatter is not null)
		{
			UseFormatter = true;
			Formatter = formatter;
		}
		if(minimumLogLevel is not null)
		{
			MinimumFileLogLevel = minimumLogLevel.Value;
		}
		return this;
	}

	public NekaiLoggerConfiguration WithJsonFileOutput(string? filePath = null, LogEventLevel? minimumLogLevel = null)
		=> _WithJsonFileOutput(false, filePath, minimumLogLevel);

	public NekaiLoggerConfiguration WithCompactJsonFileOutput(string? filePath = null, LogEventLevel? minimumLogLevel = null)
		=> _WithJsonFileOutput(true, filePath, minimumLogLevel);

	private NekaiLoggerConfiguration _WithJsonFileOutput(bool useCompactJson, string? filePath = null, LogEventLevel? minimumLogLevel = null)
	{
		LogToFile = true;
		return WithFileOutput(
			formatter: useCompactJson ? new CompactJsonFormatter() : new JsonFormatter(),
			filePath: filePath,
			minimumLogLevel: minimumLogLevel
		);
	}

	public NekaiLoggerConfiguration WithFormat(ITextFormatter format)
	{
		UseFormatter = true;
		Formatter = format;
		return this;
	}

	public NekaiLoggerConfiguration WithFormat(string format)
	{
		UseFormatter = false;
		StringFormat = format;
		return this;
	}

	public NekaiLoggerConfiguration ToggleFormat(bool useFormatter)
	{
		UseFormatter = useFormatter;
		return this;
	}
}



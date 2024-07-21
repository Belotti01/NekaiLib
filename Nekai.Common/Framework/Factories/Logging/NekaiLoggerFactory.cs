using System.Diagnostics;
using Serilog;
using ILogger = Serilog.ILogger;

namespace Nekai.Common;

// TODO: Implement ILoggerFactory.
public sealed class NekaiLoggerFactory
{
	// More simplified methods can ba added (TryCreateConsoleLogger, TryCreateFileLogger, etc)

	public Result<ILogger, OperationResult> TryCreate(NekaiLoggerConfiguration? config = null)
	{
		config ??= new NekaiLoggerConfiguration();  // Load default values

		bool isAnyOutputEnabled = config.LogToConsole || config.LogToFile;
		if(!isAnyOutputEnabled)
			return new(OperationResult.InvalidParameter);

		// Convert the simplified NekaiLoggerConfiguration to Serilog's Configuration type
		LoggerConfiguration serilogConfig = new();
		if(config.LogToConsole)
		{
			serilogConfig.WriteTo.Sink<NekaiConsoleLogEventSink>(config.MinimumConsoleLogLevel);
		}

		if(config.LogToFile)
		{
			PathString path = PathString.Parse(config.OutputFilePathTemplate);
			var result = path.GetContainingDirectory().EnsureExistsAsDirectory();
			if(result != PathOperationResult.Success)
				return new(OperationResult.InvalidParameter);

			// One overload takes an ITextFormatter, the other takes a string defining the "output template".
			// Pick one based on the configuration.
			if(config.UseFormatter)
			{
				serilogConfig.WriteTo.File(
					path: config.OutputFilePathTemplate,
					rollingInterval: RollingInterval.Day,
					restrictedToMinimumLevel: config.MinimumFileLogLevel,
					formatter: config.Formatter,
					shared: true,
					retainedFileCountLimit: 100
				);
			}
			else
			{
				serilogConfig.WriteTo.File(
					path: config.OutputFilePathTemplate,
					rollingInterval: RollingInterval.Day,
					restrictedToMinimumLevel: config.MinimumFileLogLevel,
					outputTemplate: config.StringFormat,
					shared: true
				);
			}

			serilogConfig.Enrich.WithMachineName();
		}

		ILogger logger = serilogConfig.CreateLogger();
		return new(logger);
	}

	/// <summary>
	/// Create a Serilog <see cref="ILogger"/> that writes all logs under the program's "Logs" subfolder.
	/// </summary>
	public ILogger CreateForDebug()
	{
		string path = Path.Combine(Environment.CurrentDirectory, "Logs");
		Directory.CreateDirectory(path);

		LoggerConfiguration config = new();
		config.WriteTo.File(
			path: path,
			rollingInterval: RollingInterval.Day,
			restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Debug,
			shared: true
		);

		var result = TryCreate();
		Debug.Assert(result.IsSuccessful);
		return result.Value;
	}
}
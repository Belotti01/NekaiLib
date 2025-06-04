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
	/// Create an <see cref="ILogger"/> that writes all logs under the program's "Logs" subfolder.
	/// </summary>
	/// <exception cref="PathOperationException">Thrown when the <see cref="logsDirectory"/> is <see langword="null"/> and a valid path can't be generated.</exception>
	public ILogger CreateForDebug(PathString? logsDirectory = null)
	{
		if(logsDirectory is null)
		{
			// Use the current folder + "/Logs".
			string path = Path.Combine(Environment.CurrentDirectory, "Logs");
			var pathResult = PathString.TryParse(path);
			if(!pathResult.IsSuccessful)
				pathResult.Error.Throw(path);
			
			logsDirectory = pathResult.Value;
		}
		
		Directory.CreateDirectory(logsDirectory);
		LoggerConfiguration config = new();
		config.WriteTo.File(
			path: logsDirectory,
			rollingInterval: RollingInterval.Day,
			restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Debug,
			shared: true
		);

		var result = TryCreate();
		Debug.Assert(result.IsSuccessful);
		return result.Value;
	}
}
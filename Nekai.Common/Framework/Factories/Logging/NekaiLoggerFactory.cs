using Serilog;

namespace Nekai.Common;

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
			serilogConfig.WriteTo.Console(
				restrictedToMinimumLevel: config.MinimumConsoleLogLevel,
				// The Console will always use the string formatter
				outputTemplate: config.StringFormat
			);
		}

		if(config.LogToFile)
		{
			var result = NekaiDirectory.TryEnsureExistsForFile(config.OutputFilePathTemplate);
			if(!result.IsSuccess())
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
					shared: true
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
		}

		ILogger logger = serilogConfig.CreateLogger();
		return new(logger);
	}
}

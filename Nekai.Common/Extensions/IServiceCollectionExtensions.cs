using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Nekai.Common;

public static class IServiceCollectionExtensions
{
	/// <summary>
	/// Injects the <see cref="NekaiLogs.Program"/> as an <see cref="ILogger"/> singleton.
	/// </summary>
	public static IServiceCollection AddNekaiAppLogger(this IServiceCollection services)
	{
		return services.AddSingleton(NekaiLogs.Program);
	}

	/// <summary>
	/// Inject an <see cref="ILogger"/> that logs to the current folder's "Logs" subfolder.
	/// </summary>
	/// <param name="services"> The collection of injected services. </param>
	/// <exception cref="PathOperationException"> Thrown when the current path can't be converted to a <see cref="PathString"/>. </exception>
	public static IServiceCollection AddDebugLoggingToCurrentFolder(this IServiceCollection services)
	{
		string rawPath = Path.Combine(Environment.CurrentDirectory, "Logs");
		var pathResult = PathString.TryParse(rawPath);
		if(!pathResult.IsSuccessful)
			pathResult.Error.Throw(rawPath);

		var path = pathResult.Value;
		bool pathExists = path.EnsureExistsAsDirectory().IsSuccessful();
		Debug.Assert(pathExists);

		var logger = NekaiLogs.Factory.CreateForDebug();
		return services.AddSingleton(logger);
	}
}
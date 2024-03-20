using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Core;

namespace Nekai.Common.Extensions;

public static class IServiceCollectionExtensions
{
	/// <summary>
	/// Injects the <see cref="NekaiLogs.Program"/> as an <see cref="ILogger"/> singleton.
	/// </summary>
	public static IServiceCollection AddNekaiAppLogger(this IServiceCollection services)
	{
		return services.AddSingleton(NekaiLogs.Program);
	}

	public static IServiceCollection AddDebugLoggerToCurrentFolder(this IServiceCollection services)
	{
		string rawPath = Path.Combine(Environment.CurrentDirectory, "Logs");
		var path = PathString.Parse(rawPath);
		bool pathExists = path.EnsureExistsAsDirectory().IsSuccess();
		Debug.Assert(pathExists);

		var logger = NekaiLogs.Factory.CreateForDebug();
		return services.AddSingleton(logger);
	}
}

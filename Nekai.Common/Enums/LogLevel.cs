using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Nekai.Common;

public static class LogLevelExtensions
{
	// Make sure that all LogTypes are present in this dictionary, WITH STRINGS OF EQUAL LENGTH.
	// This is important to keep the log files properly formatted for readability and parsing.
	public static IImmutableDictionary<LogLevel, string> LogCategories { get; } = new Dictionary<LogLevel, string>
	{
		{ LogLevel.None,		"     " },
		{ LogLevel.Trace,		"TRACE" },
		{ LogLevel.Debug,		"DEBUG" },
		{ LogLevel.Information, "INFO " },
		{ LogLevel.Warning,		"WARN " },
		{ LogLevel.Error,		"ERROR" },
		{ LogLevel.Critical,    "FATAL" }
	}.ToImmutableDictionary();

	static LogLevelExtensions()
	{
		// Make sure all LogTypes have an associated category string
		Debug.Assert(LogCategories.Count == Enum.GetValues(typeof(LogLevel)).Length);
		// Ensure that all category strings are of the same length
		Debug.Assert(LogCategories.Values.All(x => x.Length == LogCategories.Values.First().Length));
	}

	public static string ToLogCategory(this LogLevel type)
	{
		if(!LogCategories.TryGetValue(type, out string? category))
		{
			Exceptor.ThrowIfDebug($"No category string found for {nameof(LogLevel)} \"{type}\".");
			return type.ToString().ToLength(5);
		}
		return category;
	}

	public static ConsoleColor ToConsoleColor(this LogLevel type)
	{
		return type switch
		{
			LogLevel.Trace => ConsoleColor.Gray,
			LogLevel.Warning => ConsoleColor.Yellow,
			LogLevel.Error => ConsoleColor.Red,
			LogLevel.Critical => ConsoleColor.DarkRed,
			LogLevel.Debug => ConsoleColor.DarkGray,
			_ => ConsoleColor.White
		};
	}
}
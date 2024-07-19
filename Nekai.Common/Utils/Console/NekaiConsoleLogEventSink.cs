using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog.Core;
using Serilog.Events;

namespace Nekai.Common;

/// <summary>
/// Console logging sink that uses NekaiConsole instead of .NET's Console class.
/// </summary>
public class NekaiConsoleLogEventSink : ILogEventSink
{
	/// <inheritdoc />
	public void Emit(LogEvent logEvent)
	{
		var message = logEvent.RenderMessage();

		var timeFormat = CultureInfo.CurrentCulture.DateTimeFormat.UniversalSortableDateTimePattern;
		var timeString = logEvent.Timestamp.ToString(timeFormat);

		string output = $"[{timeString}] {message}";
		var outputColor = logEvent.Level switch
		{
			LogEventLevel.Fatal => ConsoleColor.Black,
			LogEventLevel.Error => ConsoleColor.Red,
			LogEventLevel.Warning => ConsoleColor.Yellow,
			LogEventLevel.Information => ConsoleColor.Cyan,
			_ => ConsoleColor.White
		};

		var backColor = logEvent.Level switch
		{
			LogEventLevel.Fatal => ConsoleColor.Red,
			_ => ConsoleColor.Black
		};

		NekaiConsole.WriteLine(output, outputColor, backColor);
	}
}

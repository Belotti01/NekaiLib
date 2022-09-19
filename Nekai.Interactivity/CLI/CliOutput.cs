using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nekai.Interactivity;

// Contains methods for both Output and Input.
// Input methods can be found in: CLI/CliInput.cs
public static partial class Cli {

	public static void Write(object? message, ConsoleColor textColor = ConsoleColor.White, bool debugOnly = false) {
		if(debugOnly && !Debugger.IsAttached)
			return;
		lock(_lock) {
			Console.ForegroundColor = textColor;
			Console.Write(message);
			Console.ResetColor();
		}
	}

	public static void Write(object? message, CliMessageType type, bool debugOnly = false)
		=> Write(message, _GetColor(type), debugOnly);

	public static void WriteLine(object? message = null, ConsoleColor textColor = ConsoleColor.White, bool debugOnly = false) {
		if(debugOnly && !Debugger.IsAttached)
			return;
		lock(_lock) {
			Console.ForegroundColor = textColor;
			Console.WriteLine(message);
			Console.ResetColor();
		}
	}

	public static void WriteLine(object? message, CliMessageType type, bool debugOnly = false)
		=> WriteLine(message, _GetColor(type), debugOnly);



	public static void WriteDivisor(int length)
		=> WriteDivisor(Configuration.DefaultDivisorCharacter, length);

	public static void WriteDivisor(char pattern)
		=> WriteDivisor(pattern, Configuration.DefaultMinimumDivisorLength);

	public static void WriteDivisor(char pattern, int length) {
		string div = new(pattern, length);
		WriteLine(div);
	}



	public static void WriteError(object? message, bool debugOnly = false) => WriteLine(message, Configuration.ErrorColor, debugOnly);
	public static void WriteSuccess(object? message, bool debugOnly = false) => WriteLine(message, Configuration.SuccessColor, debugOnly);
	public static void WriteWarning(object? message, bool debugOnly = false) => WriteLine(message, Configuration.WarningColor, debugOnly);
	public static void WriteInformation(object? message, bool debugOnly = false) => WriteLine(message, Configuration.InformationColor, debugOnly);
}

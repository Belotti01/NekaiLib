using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nekai.Interactivity;

// Contains general utilities for the Console.
// See CLI/CliInput.cs and CLI/CliOutput.cs for the Console interaction methods.
public static partial class Cli {
	public static CliConfiguration Configuration { get; set; } = new();

	/* Use synclock to avoid weird behaviour like: 
	 * - Mixed/Wrong colors
	 * - Mixed output text
	 * - Output being written while awaiting user input
	 */
	private static readonly object _lock = new();
	/// <summary>
	/// Whether the CLI is currently awaiting input or printing output.
	/// </summary>
	public static bool IsBusy => Monitor.IsEntered(_lock);
	/// <summary>
	/// Whether the cursor in the CLI is curently occupying the left-most column.
	/// </summary>
	public static bool IsCurrentLineEmpty => Console.CursorLeft == 0;



	/// <summary>
	/// Interrupt execution until the CLI receives all requested inputs and prints all queued outputs.
	/// </summary>
	public static void WaitUntilAvailable() {
		Monitor.Wait(_lock);
	}

	/// <summary>
	/// Delete all CLI text displayed.
	/// </summary>
	public static void Clear() {
		Console.Clear();
	}

	/// <summary>
	/// Delete the last CLI line just above the cursor.
	/// </summary>
	public static void DeleteLine(bool skipEmpty = true) {
		// If the current line is empty, move to the previous line and delete that instead
		if(Console.CursorLeft == 0) {
			if(skipEmpty && Console.CursorTop > 0)
				Console.CursorTop -= 1;
		}
		if(Console.CursorTop == 0)
			return; // Nothing to delete
		_DeleteLine();
	}

	/// <summary>
	/// Delete up to <paramref name="count"/> CLI lines, starting from the one just above the cursor.
	/// </summary>
	/// <param name="count">The amount of lines to delete.</param>
	public static void DeleteLines(int count, bool skipEmpty = true) {
		if(count <= 0)
			return; // Requeste deletion of no lines
		
		if(count >= Console.CursorTop) {
			Console.Clear();	// Requested deletion of all lines
			return;
		}

		// If the current line is empty, don't count it as to-be deleted, as nothing actually gets overwritten.
		if(skipEmpty && Console.CursorLeft == 0) {
			Console.CursorTop -= 1;
		}
		
		for(int i = 0; i < count; i++) {
			_DeleteLine();
			Console.CursorTop -= 1;
		}
		_DeleteLine();
	}



	private static void _DeleteLine() {
		int length = Console.BufferWidth - 1;
		Console.CursorLeft = 0;
		//for(int i = 0; i < length; i++)
		Console.Write(new string(' ', length));
		Console.CursorLeft = 0;
	}

	private static ConsoleColor _GetColor(CliMessageType forType)
		=> forType switch {
			CliMessageType.Success => Configuration.SuccessColor,
			CliMessageType.Information => Configuration.InformationColor,
			CliMessageType.Warning => Configuration.WarningColor,
			CliMessageType.Error => Configuration.ErrorColor,
			_ => ConsoleColor.White
		};
}

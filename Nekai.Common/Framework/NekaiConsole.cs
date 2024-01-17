﻿using System;

namespace Nekai.Common;

/// <summary>
/// Thread-safe Console output and input methods.
/// </summary>
public static class NekaiConsole
{
	private static readonly object _lock = new();
	
	/// <summary>
	/// Print the framework signature to the console with the specified number of tabs.
	/// </summary>
	/// <param name="tabs"> How many tabs to prepend to each line of the signature. </param>
	/// <param name="textColor"> The color of the signature text. </param>
	public static void PrintSignature(int tabs = 0, ConsoleColor textColor = ConsoleColor.Green)
	{
		string formattedSignature = NekaiString.Signature.Tabbed(tabs);
		
		WriteLine(formattedSignature, textColor);
		WriteLine();
	}

	public static void Write(object? obj, ConsoleColor textColor = ConsoleColor.White)
	{
		lock(_lock)
		{
			ConsoleColor lastColor = Console.ForegroundColor;
			Console.ForegroundColor = textColor;

			Console.Write(obj);

			Console.ForegroundColor = lastColor;
		}
	}

	public static void WriteLine(object? obj, ConsoleColor textColor = ConsoleColor.White)
	{
		lock(_lock)
		{
			ConsoleColor lastColor = Console.ForegroundColor;
			Console.ForegroundColor = textColor;

			Console.WriteLine(obj);

			Console.ForegroundColor = lastColor;
		}
	}

	public static void WriteLine()
	{
		lock(_lock)
		{
			Console.WriteLine();
		}
    }

    public static void WriteAtPosition(object? obj, int left, int top, ConsoleColor textColor = ConsoleColor.White)
    {
        lock(_lock)
        {
            // Move cursor to the next loading character.
            var lastPosition = Console.GetCursorPosition();
            Console.SetCursorPosition(left, top);

			var oldColor = Console.ForegroundColor;
			Console.ForegroundColor = textColor;
            Console.Write(obj);
			Console.ForegroundColor = oldColor;

            // Move cursor back to its original position.
            Console.SetCursorPosition(lastPosition.Left, lastPosition.Top);
        }
    }

	public static ConsoleLoadingBuilder CreateDotLoader()
		=> new();
}

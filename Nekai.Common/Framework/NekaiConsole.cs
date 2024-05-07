using System;
using System.Numerics;

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
    public static void PrintSignature(int tabs = 0, ConsoleColor textColor = ConsoleColor.Green, ConsoleColor backColor = ConsoleColor.Black)
    {
        string formattedSignature = NekaiString.Signature.Tabbed(tabs);

        lock(_lock)
        {
            _WriteLine(formattedSignature, textColor, backColor);
            Console.WriteLine();
        }
    }

	public static void Write(object? obj, ConsoleColor textColor = ConsoleColor.White, ConsoleColor backColor = ConsoleColor.Black)
	{
		lock(_lock)
		{
			_Write(obj, textColor, backColor);
		}
	}

	public static void WriteLine(object? obj, ConsoleColor textColor = ConsoleColor.White, ConsoleColor backColor = ConsoleColor.Black)
	{
		lock(_lock)
		{
			_WriteLine(obj, textColor, backColor);
		}
	}

	public static void WriteLine()
	{
		lock(_lock)
		{
			Console.WriteLine();
		}
    }

    public static void WriteError(object? obj, ConsoleColor textColor = ConsoleColor.White)
        => WriteLine(obj, textColor, ConsoleColor.Red);

    public static void WriteWarning(object? obj, ConsoleColor textColor = ConsoleColor.Black)
        => WriteLine(obj, textColor, ConsoleColor.Yellow);

    public static void WriteSuccess(object? obj, ConsoleColor textColor = ConsoleColor.Black)
        => WriteLine(obj, textColor, ConsoleColor.Green);

    public static void WriteAtPosition(object? obj, int left, int top, ConsoleColor textColor = ConsoleColor.White)
    {
        lock(_lock)
        {
            // Move cursor to the next loading character.
            var lastPosition = Console.GetCursorPosition();
            Console.SetCursorPosition(left, top);

			_Write(obj, textColor);

            // Move cursor back to its original position.
            Console.SetCursorPosition(lastPosition.Left, lastPosition.Top);
        }
    }

	public static T Read<T>()
		where T : IParsable<T>
	{
		lock(_lock)
		{
			T? result = default;
			string? input;
			bool isValidInput = false;
			
			do
			{
				input = Console.ReadLine();
				isValidInput = T.TryParse(input, null, out result);
            } while(!isValidInput);

			return result!;
		}
	}

    public static string ReadLine()
	{
		return Read<string>();
    }

    public static ConsoleKeyInfo ReadKey(ConsoleKey[]? allowed = null)
    {
        lock(_lock)
        {
            if(allowed is null)
                return Console.ReadKey();

            ConsoleKeyInfo input;

            do
            {
                input = Console.ReadKey();
            } while(!allowed.Contains(input.Key));

            return input;
        }
    }

    public static char ReadChar()
    {
        lock(_lock)
        {
            int input;

            do
            {
                input = Console.Read();
            } while(input == -1);

            return (char)input;
        }
    }

    public static char ReadChar(char[] allowed)
    {
        lock(_lock)
        {
            int input;

            do
            {
                input = Console.Read();
            } while(input == -1 || !allowed.Contains((char)input));

            return (char)input;
        }
    }

    public static ConsoleLoadingBuilder CreateDotLoader()
		=> new();

    /// <summary> Non-locking nternal wrapper of the <see cref="Console.Write()"/> operation. </summary>
    private static void _Write(object? text, ConsoleColor foreColor = ConsoleColor.White, ConsoleColor backColor = ConsoleColor.Black)
    {
        var prevForeColor = Console.ForegroundColor;
        var prevBackColor = Console.BackgroundColor;

        Console.ForegroundColor = foreColor;
        Console.BackgroundColor = backColor;

        Console.Write(text);

        Console.ForegroundColor = prevForeColor;
        Console.BackgroundColor = prevBackColor;
    }

    /// <summary> Non-locking nternal wrapper of the <see cref="Console.WriteLine()"/> operation. </summary>
    private static void _WriteLine(object? text, ConsoleColor foreColor = ConsoleColor.White, ConsoleColor backColor = ConsoleColor.Black)
    {
        var prevForeColor = Console.ForegroundColor;
        var prevBackColor = Console.BackgroundColor;

        Console.ForegroundColor = foreColor;
        Console.BackgroundColor = backColor;

        Console.WriteLine(text);

        Console.ForegroundColor = prevForeColor;
        Console.BackgroundColor = prevBackColor;
    }
}


namespace Nekai.Common;

/// <summary>
/// Thread-safe Console output and input methods.
/// </summary>
public static class NekaiConsole
{
	/// <summary>
	/// Whether to print out each character one by one, rather than the whole string in one shot.
	/// </summary>
	public static bool SlowPrintMode { get; set; }
	/// <summary>
	/// The delay between each character when writing in <see cref="SlowPrintMode"/>.
	/// </summary>
	/// <remarks>
	/// Defaults to 30ms.
	/// </remarks>
	public static TimeSpan SlowPrintDelay = TimeSpan.FromMilliseconds(20);
	
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

	public static T Read<T>(IFormatProvider? format = null)
		where T : IParsable<T>
	{
		T? result;
		string? input;
		bool isValidInput;

		lock(_lock)
		{
			do
			{
				input = Console.ReadLine();
				isValidInput = T.TryParse(input, format, out result);
			} while(!isValidInput);
		}

		return result!;
	}

	public static string ReadLine() => Read<string>();

	public static ConsoleKeyInfo ReadKey(params ConsoleKey[] allowed)
	{
		lock(_lock)
		{
			if(allowed.Length == 0)
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
		int input;

		lock(_lock)
		{
			do
			{
				input = Console.Read();
			} while(input == -1);
		}
		return (char)input;
	}

	public static char ReadChar(params char[] allowed)
	{
		int input;
		bool isValidInput;

		lock(_lock)
		{
			do
			{
				input = Console.Read();
				isValidInput = input == -1
					|| !allowed.Contains((char)input);
			} while(isValidInput);
		}
		return (char)input;
	}

	public static ConsoleLoadingBuilder CreateDotLoader()
		=> new();

	/// <summary> Non-locking internal wrapper of the <see cref="Console.Write(object)"/> operation. </summary>
	private static void _Write(object? text, ConsoleColor foreColor = ConsoleColor.White, ConsoleColor backColor = ConsoleColor.Black)
	{
		var prevForeColor = Console.ForegroundColor;
		var prevBackColor = Console.BackgroundColor;

		Console.ForegroundColor = foreColor;
		Console.BackgroundColor = backColor;

		string? s = text?.ToString();

		if(SlowPrintMode && s is not null)
		{
			foreach(var c in s)
			{
				Console.Write(c);
				Thread.Sleep(SlowPrintDelay);
			}
		}
		else
		{
			Console.Write(text);
		}
		Console.ForegroundColor = prevForeColor;
		Console.BackgroundColor = prevBackColor;
	}

	/// <summary> Non-locking internal wrapper of the <see cref="Console.WriteLine()"/> operation. </summary>
	private static void _WriteLine(object? text, ConsoleColor foreColor = ConsoleColor.White, ConsoleColor backColor = ConsoleColor.Black)
	{
		var prevForeColor = Console.ForegroundColor;
		var prevBackColor = Console.BackgroundColor;

		Console.ForegroundColor = foreColor;
		Console.BackgroundColor = backColor;

		string? s = text?.ToString();

		if(SlowPrintMode && s is not null)
		{
			foreach(var c in s)
			{
				Console.Write(c);
				Thread.Sleep(SlowPrintDelay);
			}
			Console.WriteLine();
		}
		else
		{
			Console.WriteLine(text);
		}

		Console.ForegroundColor = prevForeColor;
		Console.BackgroundColor = prevBackColor;
	}
}
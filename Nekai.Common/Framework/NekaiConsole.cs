namespace Nekai.Common;

public static class NekaiConsole
{
	private static readonly object _lock = new();


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
}

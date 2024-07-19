namespace Nekai.Common;

public class ConsoleLoadingBuilder
{
	public string Prefix { get; set; } = "";
	public string Postfix { get; set; } = "";
	public char Character { get; set; } = '.';
	public TimeSpan Interval { get; set; } = TimeSpan.FromSeconds(1);
	private int _maxCharacters = 3;
	public int MaxCharacters { 
		get => _maxCharacters; 
		set => _maxCharacters = int.Max(0, value);
	}
	public ConsoleColor Color { get; set; } = ConsoleColor.White;


	public async Task RunAsync(CancellationToken token = default)
	{
		if(MaxCharacters == 0 || token.IsCancellationRequested)
			return;

		var loadingPosition = Console.GetCursorPosition();

		int index = Prefix.Length;

		string startingLoadingBar = Prefix + Character + new string(' ', MaxCharacters - 1) + Postfix;

		// Prepares the postfix in place, otherwise it would appear only after the first MaxCharacters iterations.
		NekaiConsole.WriteAtPosition(startingLoadingBar, loadingPosition.Left + Prefix.Length, loadingPosition.Top);

		while(!token.IsCancellationRequested)
		{
			if(index > MaxCharacters)
			{
				index = 1;
				NekaiConsole.WriteAtPosition(startingLoadingBar, loadingPosition.Left + Prefix.Length, loadingPosition.Top);
			}
			else
			{
				NekaiConsole.WriteAtPosition(Prefix + new string(Character, index), loadingPosition.Left + Prefix.Length, loadingPosition.Top, Color);
			}
			index++;

			await Task.Delay(Interval, token);
		}
	}

	public void Run(CancellationToken token = default)
	{
		var task = RunAsync(token);
		task.Wait();
	}
}
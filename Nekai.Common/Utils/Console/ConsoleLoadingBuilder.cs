namespace Nekai.Common;

public class ConsoleLoadingBuilder
{
	public char Character { get; private set; } = '.';
	public TimeSpan Interval { get; private set; } = TimeSpan.FromSeconds(1);
	public int MaxCharacters { get; private set; } = 3;
	public ConsoleColor Color { get; private set; } = ConsoleColor.White;

	public ConsoleLoadingBuilder WithCharacter(char character)
	{
		Character = character;
		return this;
	}

	public ConsoleLoadingBuilder WithInterval(TimeSpan interval)
	{
		if(interval == default)
			interval = TimeSpan.FromSeconds(1);
		Interval = interval;
		return this;
	}

	public ConsoleLoadingBuilder WithMaxCharacters(int maxCharacters)
	{
		MaxCharacters = int.Max(maxCharacters, 0);
		return this;
	}

	public ConsoleLoadingBuilder WithColor(ConsoleColor color)
	{
		Color = color;
		return this;
	}

	public async Task RunAsync(CancellationToken token = default)
	{
		if(MaxCharacters == 0 || token.IsCancellationRequested)
			return;

		var loadingPosition = Console.GetCursorPosition();

		int index = 0;

		string startingLoadingBar = Character + new string(' ', MaxCharacters - 1);

		while(!token.IsCancellationRequested)
		{
			if(index > MaxCharacters)
			{
				index = 1;
				NekaiConsole.WriteAtPosition(startingLoadingBar, loadingPosition.Left, loadingPosition.Top);
			}
			else
			{
				NekaiConsole.WriteAtPosition(new string(Character, index), loadingPosition.Left, loadingPosition.Top, Color);
			}
			index++;

			await Task.Delay(Interval, token);
		}
	}

	public void Run(CancellationToken token = default)
	{
		RunAsync(token)
			.RunSynchronously();
	}
}
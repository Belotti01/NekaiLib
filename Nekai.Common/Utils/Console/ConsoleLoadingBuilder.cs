namespace Nekai.Common;

public class ConsoleLoadingBuilder
{
	/// <summary> The loading bar's prefix. </summary>
	public string Prefix { get; set; } = "";
	/// <summary> The loading bar's postfix. </summary>
	public string Postfix { get; set; } = "";
	/// <summary> The character indicating a filled space of the loading bar. Defaults to '.'. </summary>
	public char Character { get; set; } = '.';
	/// <summary> The interval between each update. Defaults to 1 second. </summary>
	public TimeSpan Interval { get; set; } = TimeSpan.FromSeconds(1);
	private int _maxCharacters = 3;
	/// <summary> The amount of <see cref="Character"/>s indicating a filled loading bar. </summary>
	public int MaxCharacters
	{
		get => _maxCharacters; 
		set => _maxCharacters = int.Max(0, value);
	}
	/// <summary> The color of the loading bar. </summary>
	public ConsoleColor Color { get; set; } = ConsoleColor.White;

	/// <summary>
	/// Asynchronously build, print, and keep updating the loading bar until the <paramref name="token"/> emits cancellation.
	/// </summary>
	/// <param name="token"> The token used to stop the updates. </param>
	public async Task RunAsync(CancellationToken token = default)
	{
		if(MaxCharacters == 0 || token.IsCancellationRequested)
			return;

		var loadingPosition = Console.GetCursorPosition();

		int index = Prefix.Length;

		string startingLoadingBar = Prefix + Character + new string(' ', MaxCharacters - 1) + Postfix;

		// Prepares the postfix in place, otherwise it would appear only after the first MaxCharacters iterations.
		NekaiConsole.WriteAtPosition(startingLoadingBar, loadingPosition.Left + Prefix.Length, loadingPosition.Top);
		// Set the pointer to the next line, as to avoid printing over the loading bar.
		NekaiConsole.WriteLine();

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

	/// <summary>
	/// Build, print, and keep updating the loading bar until the <paramref name="token"/> emits cancellation.
	/// </summary>
	/// <param name="token"> The token used to stop the updates. </param>
	public void Run(CancellationToken token = default)
	{
		var task = RunAsync(token);
		task.Wait(token);
	}
}
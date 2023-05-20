namespace Nekai.Interactivity.Commands;

public interface ICommand<ICommandData>
{
	ICommandData Data { get; }

	public void Execute();
}
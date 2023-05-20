namespace Nekai.Interactivity.Commands;

public interface ICommandManager<TCommandList, TCommand, TCommandArgument>
	where TCommandList : ICommandList<TCommand>
	where TCommand : ICommandData
	where TCommandArgument : ICommandArgument
{
	public TCommandList Commands { get; }

	public void Execute(string commandLine);
}
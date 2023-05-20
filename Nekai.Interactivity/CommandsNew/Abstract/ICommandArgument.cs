namespace Nekai.Interactivity.Commands;

public interface ICommandArgument
{
	string[] Aliases { get; }
	string? Description { get; }
}
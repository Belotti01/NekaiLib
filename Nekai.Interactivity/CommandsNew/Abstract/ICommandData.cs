namespace Nekai.Interactivity.Commands;

public interface ICommandData
{
	string[] Aliases { get; }
	string? Description { get; }
}
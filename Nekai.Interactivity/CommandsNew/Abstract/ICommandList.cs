using System.Diagnostics.CodeAnalysis;

namespace Nekai.Interactivity.Commands;

public interface ICommandList<ICommand>
{
	public ulong Count { get; }
	public ICommand? this[string alias] { get; }

	public bool TryGetCommand(string alias, [NotNullWhen(true)] out ICommand? command);
}
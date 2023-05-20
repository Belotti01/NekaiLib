using System.Reflection;

namespace Nekai.Interactivity.Commands;

public abstract class Command<TCommandData> : ICommand<TCommandData>
	where TCommandData : CommandAttribute
{
	/// <summary>
	/// Information regarding the command.
	/// </summary>
	public TCommandData Data { get; private init; }

	/// <summary>
	/// Initialize an instance of <see cref="Command{TCommandData}"/>, retrieving the data from the
	/// attribute of type <typeparamref name="TCommandData"/>.
	/// </summary>
	/// <exception cref="MissingAttributeException">Missing Command Data definition as attribute of type <see cref="TCommandData"/>.</exception>
	public Command()
	{
		var data = GetType().GetCustomAttribute<TCommandData>();
		if(data is null)
			throw new MissingAttributeException(typeof(TCommandData), GetType());

		Data = data;
	}

	/// <summary>
	/// Execute the command with the loaded arguments.
	/// </summary>
	public abstract void Execute();
}
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Nekai.Interactivity;

public interface ICommandsManager<TCommand, TCommandAttribute, TArgumentAttribute>
	where TCommand : ICommand<TCommand, TCommandAttribute, TArgumentAttribute>
	where TCommandAttribute : CommandAttribute
	where TArgumentAttribute : CommandArgumentAttribute
{
	/// <summary> Retrieve the amount of Commands loaded within this object. </summary>
	int CommandsCount { get; }

	/// <summary>
	/// Get the Command that should be executed by the given <paramref name="commandStringOrAlias"/>.
	/// </summary>
	/// <param name="commandStringOrAlias"> The alias of the command to retrieve, optionally followed by arguments. </param>
	/// <returns> The <see cref="TCommand"/> instance retrieved, or <see langword="null"/> if none is found. </returns>
	TCommand? GetCommand(string commandStringOrAlias);

	/// <summary>
	/// Build a complete "Help" message with the available information of all the loaded Commands.
	/// </summary>
	/// <returns> A formatted <see langword="string"/> listing all Commands' aliases, descriptions and arguments. </returns>
	string GetHelpMessage(bool includeArguments);

	/// <summary>
	/// Load all Commands with Attribute <typeparamref name="TCommandAttribute"/> from the given <paramref name="assembly"/>.
	/// </summary>
	/// <param name="assembly"> The Assembly containing the Commands' Types definitions. </param>
	void LoadCommands(Assembly assembly);

	/// <summary>
	/// Attempt to retrieve a Command that can be identified by the given <paramref name="commandAlias"/>.
	/// </summary>
	/// <param name="commandAlias"> [Case-Insensitive] One of the Aliases of the Command to retrieve. </param>
	/// <param name="command"> The retrieved Command if one is found, <see langword="null"/> otherwise. </param>
	/// <returns> <see langword="true"/> if a <paramref name="command"/> is found, <see langword="false"/> otherwise. </returns>
	bool TryGetCommand(string commandAlias, [NotNullWhen(true)] out TCommand? command);

	/// <summary>
	/// Attempt to retrieve a Command that can be identified by the given <paramref name="commandString"/>, extracting
	/// excess Arguments into <paramref name="args"/>.
	/// </summary>
	/// <param name="commandString"> [Case-Insensitive] One of the Aliases of the Command to retrieve, optionally followed by
	/// Arguments that will be extracted into <paramref name="args"/>. </param>
	/// <param name="command"> The retrieved Command if one is found, <see langword="null"/> otherwise. </param>
	/// <param name="args"> The Arguments following the Command's alias in the given <paramref name="commandString"/>. </param>
	/// <returns> <see langword="true"/> if a <paramref name="command"/> is found, <see langword="false"/> otherwise. </returns>
	bool TryGetCommand(string commandString, [NotNullWhen(true)] out TCommand? command, out string args);
}

namespace Nekai.Interactivity;

public interface ICommand<TCommand, TCommandAttribute, TArgumentAttribute> 
	where TCommand : CommandBase
	where TCommandAttribute : CommandAttribute
	where TArgumentAttribute : CommandArgumentAttribute {
	/// <summary>
	/// All names attributed to this Command.
	/// </summary>
	string[] Aliases { get; }
	/// <summary>
	/// Information regarding the Arguments appliable to this Command.
	/// </summary>
	TArgumentAttribute[] ArgumentsInfo { get; }
	/// <summary>
	/// The <see cref="Type"/> that defines the behaviour of this Command.
	/// </summary>
	Type Definition { get; }
	/// <summary>
	/// The Description attributed to this Command, or <see cref="null"/> if none is set.
	/// </summary>
	string? Description { get; }

	/// <summary>
	/// Run the Command after parsing the <paramref name="args"/> and passing them to the <typeparamref name="TCommand"/>
	/// instance.
	/// </summary>
	/// <param name="args"> The Arguments to pass to the <typeparamref name="TCommand"/> before execution. </param>
	/// <returns> <see langword="true"/> if the execution method has been invoked, or <see langword="false"/> if
	/// the validation failed.</returns>
	bool Execute(string? args = null);
	/// <summary>
	/// Retrieve a "Help" message with information regarding this Command.
	/// </summary>
	/// <param name="includeArguments"> Whether to append information extracted from the <see cref="ArgumentsInfo"/> or not. </param>
	/// <returns> A formatted <see langword="string"/> containing the <see cref="Aliases"/>, <see cref="Description"/>
	/// and, if <paramref name="includeArguments"/> is <see langword="true"/>, information regarding the 
	/// <see cref="ArgumentsInfo"/> of this Command. </returns>
	string GetHelpMessage(bool includeArguments = true);
}
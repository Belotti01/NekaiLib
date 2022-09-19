namespace Nekai.Interactivity;

public abstract class CommandBase {

	public CommandBase() { }


	/// <summary>
	/// Method Invoked before executing Command, after the Arguments have been assigned.
	/// Execution will only take place if this method returns <see langword="true"/>.
	/// </summary>
	public virtual bool CanExecute() => true;
	/// <summary>
	/// Method Invoked when executing this Command, after the Arguments have been assigned and validated.
	/// </summary>
	public abstract void Execute();

}

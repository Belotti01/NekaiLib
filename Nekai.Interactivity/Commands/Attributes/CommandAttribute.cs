namespace Nekai.Interactivity;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
public class CommandAttribute : BaseCommandDataAttribute
{
	public CommandAttribute(string command, params string[] aliases) : base(command, aliases)
	{
	}
}
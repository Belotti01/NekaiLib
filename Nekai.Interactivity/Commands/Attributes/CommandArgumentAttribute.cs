namespace Nekai.Interactivity;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class CommandArgumentAttribute : BaseCommandDataAttribute
{
	public CommandArgumentAttribute(string name, params string[] aliases) : base(name, aliases)
	{
	}
}
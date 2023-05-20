namespace Nekai.Interactivity.Commands;

[AttributeUsage(AttributeTargets.Class, Inherited = true)]
public class CommandAttribute : Attribute, ICommandData
{
	public string[] Aliases { get; private init; }
	public string? Description { get; set; }

	public CommandAttribute(params string[] aliases)
	{
		Aliases = aliases;
	}
}
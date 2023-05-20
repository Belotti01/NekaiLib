using System.Collections.ObjectModel;
using System.Reflection;
using System.Text;
using Nekai.Common.Reflection;

namespace Nekai.Interactivity;

public class Command<TCommand, TCommandAttribute, TArgumentAttribute>
	: ICommand<TCommand, TCommandAttribute, TArgumentAttribute>
	where TCommand : ICommand<TCommand, TCommandAttribute, TArgumentAttribute>
	where TCommandAttribute : CommandAttribute
	where TArgumentAttribute : CommandArgumentAttribute
{
	public Type Definition { get; protected set; }
	protected ReadOnlyDictionary<MemberInfo, TArgumentAttribute> Arguments { get; private set; }
	protected TCommandAttribute Info { get; set; }

	public string[] Aliases => Info.GetAllNames();
	public string? Description => Info.Description;
	public TArgumentAttribute[] ArgumentsInfo => Arguments.Values.ToArray();

	public Command()
	{ }

	public void Initialize(Type definition, TCommandAttribute commandAttribute)
	{
		Definition = definition;
		Info = commandAttribute;

		var argumentMembers = definition.GetMembersWithAttribute<TArgumentAttribute>(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic);
		var members = definition.GetMembers();
		var allMembers = definition.GetMembers().Where(x => x.GetCustomAttribute<TArgumentAttribute>() is not null);
		Dictionary<MemberInfo, TArgumentAttribute> args = new();
		foreach(var arg in argumentMembers)
		{
			// Ignore nullability - check has already been made
			var attribute = arg.GetCustomAttribute<TArgumentAttribute>(true)!;
			args.Add(arg, attribute);
		}
		Arguments = new(args);
	}

	public bool Execute(string? args = null)
	{
		var command = (TCommand?)Definition.GetConstructor(Array.Empty<Type>())?.Invoke(null);
		if(command is null)
			return false;
		command.Initialize(Definition, Info);
		if(command is null)
			throw new InvalidTypeException(Definition, $"Type \"{Definition.Name}\" does not implement a parameterless constructor.");

		ArgumentsReader argsReader = new(args ?? "");
		Type returnType;
		foreach(var (member, argumentInfo) in Arguments)
		{
			if(!argsReader.TryReadAny(argumentInfo.GetAllNames(), out string? argString))
				continue;

			returnType = member.GetMemberReturnType();
			if(!NekaiParsing.TryParse(argString, returnType, out object? argValue))
			{
				NekaiLogs.Shared.Warning($"Could not parse serialized argument \"{argString}\" for member \"{member.Name}\" of type {returnType.Name}.");
			}
			member.SetValue(command, argValue);
		}

		if(!command.CanExecute())
			return false;
		command.Execute();
		return true;
	}

	public string GetHelpMessage(bool includeArguments = true)
	{
		StringBuilder sb = new();
		// Command Name(s)
		sb.AppendLine($"- {string.Join(", ", Info.GetAllNames())}");

		// Description
		if(!string.IsNullOrWhiteSpace(Info.Description))
		{
			sb.Append('\t')
				.AppendLine(Info.Description);
		}

		// Arguments
		if(includeArguments)
		{
			foreach(var arg in Arguments.Values)
			{
				sb.AppendLine($"\t- {string.Join("|", arg.GetAllNames())}: {arg.Description ?? "Generic parameter"}");
			}
		}

		return sb.ToString();
	}
}
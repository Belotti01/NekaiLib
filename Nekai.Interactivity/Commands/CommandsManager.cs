using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Nekai.Interactivity;

public class CommandsManager
    : CommandsManager<CommandBase, CommandAttribute, CommandArgumentAttribute>
{
    public CommandsManager() : base() { }
	
	public CommandsManager(Assembly assembly) : base(assembly) { }
}



public class CommandsManager<TCommand, TCommandAttribute, TArgumentAttribute> 
    : ICommandsManager<TCommand, TCommandAttribute, TArgumentAttribute> 
    where TCommand : CommandBase
    where TCommandAttribute : CommandAttribute
    where TArgumentAttribute : CommandArgumentAttribute {

    protected readonly List<Command<TCommand, TCommandAttribute, TArgumentAttribute>> Commands = new();
    public int CommandsCount => Commands.Count;


    public CommandsManager() { }

    public CommandsManager(Assembly assembly) : this()
    {
        LoadCommands(assembly);
    }

	/// <inheritdoc cref="ICommandsManager{TCommand, TCommandAttribute, TArgumentAttribute}.LoadCommands(Assembly)"/>
    /// <exception cref="InvalidTypeException"> Thrown when one or more of the Commands found in the <paramref name="assembly"/>
    /// does not inherit the base Type <see cref="TCommand"/>. </exception>
    public void LoadCommands(Assembly assembly)
    {
        var commandTypes = assembly
            .GetTypes()
            .Where(x
                => x.GetCustomAttribute<TCommandAttribute>() is not null);
        Commands.Clear();

        foreach(var type in commandTypes)
        {
            if(!type.IsAssignableTo(typeof(TCommand)))
                throw new InvalidTypeException(type, $"Command of Type \"{type.Name}\" must inherit base Type \"{typeof(TCommand).Name}\"");
            Commands.Add(Command<TCommand, TCommandAttribute, TArgumentAttribute>.Generate(type));
        }
    }

    public Command<TCommand, TCommandAttribute, TArgumentAttribute>? GetCommand(string commandStringOrAlias)
    {
        return Commands
            .FirstOrDefault(x
                => x.Aliases.Contains(commandStringOrAlias, StringComparer.OrdinalIgnoreCase)
                || x.Aliases.Any(alias => commandStringOrAlias.StartsWith(alias + " ")));
    }

    public bool TryGetCommand(string commandAlias, [NotNullWhen(true)] out Command<TCommand, TCommandAttribute, TArgumentAttribute>? command)
    {
        command = GetCommand(commandAlias);
        return command is not null;
    }

    public bool TryGetCommand(string commandString, [NotNullWhen(true)] out Command<TCommand, TCommandAttribute, TArgumentAttribute>? command, out string args)
    {
        args = commandString;
        command = GetCommand(commandString);
        if(command is null)
            return false;

        // Order Aliases from longest to shorted, to prioritize trimming, f.e., "Test Command" over just "Test" when possible
        foreach(var alias in command.Aliases.OrderByDescending(x => x.Length))
        {
            if(commandString.StartsWith(alias))
            {
                args = commandString[alias.Length..].TrimStart();
                break;
            }

        }
        return true;
    }

    public string GetHelpMessage(bool includeArguments)
    {
        string divisor = includeArguments ? "\n" : "";
        return "[HELP]\n" + string.Join(divisor, Commands.Select(x => x.GetHelpMessage(includeArguments)));
    }
}

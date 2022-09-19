using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text;
using Nekai.Common.Reflection;

namespace Nekai.Interactivity;

public class Command<TCommand, TCommandAttribute, TArgumentAttribute> 
    : ICommand<TCommand, TCommandAttribute, TArgumentAttribute>
    where TCommand : CommandBase
    where TCommandAttribute : CommandAttribute
    where TArgumentAttribute : CommandArgumentAttribute {


    public Type Definition { get; protected init; }
    protected ReadOnlyDictionary<MemberInfo, TArgumentAttribute> Arguments { get; private init; }
    protected CommandAttribute Info { get; init; }

    public string[] Aliases => Info.GetAllNames();
    public string? Description => Info.Description;
    public TArgumentAttribute[] ArgumentsInfo => Arguments.Values.ToArray();


    protected Command(Type commandClass, CommandAttribute commandAttribute)
    {
        Definition = commandClass;
        Info = commandAttribute;

        var argumentMembers = commandClass.GetMembersWithAttribute<TArgumentAttribute>(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic);
        var members = commandClass.GetMembers();
        var allMembers = commandClass.GetMembers().Where(x => x.GetCustomAttribute<TArgumentAttribute>() is not null);
        Dictionary<MemberInfo, TArgumentAttribute> args = new();
        foreach(var arg in argumentMembers)
        {
            // Ignore nullability - check has already been made
            var attribute = arg.GetCustomAttribute<TArgumentAttribute>(true)!;
            args.Add(arg, attribute);
        }
        Arguments = new(args);
    }

    internal static Command<TCommand, TCommandAttribute, TArgumentAttribute> Generate(Type commandType)
    {
        if(!commandType.IsAssignableTo(typeof(TCommand)))
            throw new ArgumentException($"Command Type \"{commandType.Name}\" does not inherit type \"{nameof(TCommand)}\"", nameof(commandType));
        var attribute = commandType.GetCustomAttribute<TCommandAttribute>();
        if(attribute is null)
            throw new MissingAttributeException(commandType, typeof(TCommandAttribute));

        return new(commandType, attribute);
    }

    internal static bool TryGenerate(Type commandType, [NotNullWhen(true)] out Command<TCommand, TCommandAttribute, TArgumentAttribute>? command)
    {
        command = null;
        if(!commandType.IsAssignableTo(typeof(TCommand)))
            return false;

        var attribute = commandType.GetCustomAttribute<TCommandAttribute>();
        if(attribute is null)
            return false;

        command = new(commandType, attribute);
        return true;
    }



    public bool Execute(string? args = null)
    {
        var command = (TCommand?)Definition.GetConstructor(Array.Empty<Type>())?.Invoke(null);
        if(command is null)
            throw new InvalidTypeException(Definition, $"Type \"{Definition.Name}\" does not implement a parameterless constructor.");

        ArgumentsReader argsReader = new(args ?? "");
        foreach(var (member, argumentInfo) in Arguments)
        {
            if(!argsReader.TryReadAny(argumentInfo.GetAllNames(), out string? argString))
                continue;

            var argValue = argString.ConvertTo(member.GetMemberReturnType());
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

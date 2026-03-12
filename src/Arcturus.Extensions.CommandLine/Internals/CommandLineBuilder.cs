using Arcturus.CommandLine.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.CommandLine;
using System.CommandLine.Help;
using System.Linq.Expressions;
using System.Reflection;

namespace Arcturus.CommandLine.Internals;

internal sealed class CommandLineBuilder<T>
    (CommandLineConfiguration config) where T : CommandLineRoot
{
    private readonly CommandLineConfiguration _config = config;

    // Resolved once per AppDomain — shared across all registrations
    private static readonly MethodInfo _parseResultGetValue = typeof(ParseResult)
        .GetMethods(BindingFlags.Public | BindingFlags.Instance)
        .First(m =>
            m.Name == "GetValue"
            && m.IsGenericMethodDefinition
            && m.GetParameters() is { Length: 1 } p
            && p[0].ParameterType.IsGenericType
            && p[0].ParameterType.GetGenericTypeDefinition() == typeof(Option<>));

    internal RootCommand Build(IHost host, T rootInstance, CancellationToken cancellationToken)
    {
        var commandDetails = typeof(T).GetCustomAttribute<CommandAttribute>();
        if (commandDetails is null)
            throw new InvalidOperationException($"The root command {typeof(T).FullName} must be decorated with {nameof(CommandAttribute)}.");

        var commandLineRoot = new RootCommand(commandDetails?.Description ?? "");

        AssignCommandsRecrusive(rootInstance, commandLineRoot, host.Services, cancellationToken);

        if (_config.ConfigureHelpDelegate is not null)
        {
            AssignHelpRecrusive(commandLineRoot);            
        }

        return commandLineRoot;
    }

    private void AssignHelpRecrusive(Command commandLineRoot)
    {
        foreach (var opt in commandLineRoot.Options)
        {
            if (opt is HelpOption helpOption)
            {
                helpOption.Action =
                    _config.ConfigureHelpDelegate((HelpAction)helpOption.Action!, commandLineRoot) ?? helpOption.Action;
            }
        }
        foreach (var subCommand in commandLineRoot.Subcommands)
        {
            AssignHelpRecrusive(subCommand);
        }
    }

    private void AssignCommandsRecrusive(
        IAbstractCommand parentCommand
        , Command commandLineCommand
        , IServiceProvider services
        , CancellationToken cancellationToken)
    {
        var subCommandAttributes = parentCommand.GetSubCommandAttributes();
        foreach (var subCommandAttribute in subCommandAttributes)
        {
            if (subCommandAttribute is not null && subCommandAttribute.SubCommands?.Length > 0)
            {
                foreach (var subCommandType in subCommandAttribute.SubCommands.OrderBy(_ => _.Name))
                {
                    var command = (ICommand)Activator.CreateInstance(subCommandType)!;
                    var commandDetails = subCommandType.GetCustomAttribute<CommandAttribute>();
                    if (commandDetails is null)
                        continue;

                    var wrappedCommand = new Command(commandDetails.Name, commandDetails.Description);

                    // Pre-compile one setter delegate per option — runs once at registration, not per parse
                    var optionSetters = new List<Action<ICommand, ParseResult>>();
                    foreach (var propertyOption in command.GetOptions())
                    {
                        var option = CreateOptionInstance(
                            propertyOption.PropertyInfo.PropertyType
                            , propertyOption.Option.Name
                            , propertyOption.Option.Aliases)!;

                        option.Description = propertyOption.Option.Description;
                        option.Required = propertyOption.PropertyInfo.IsRequiredProperty();

                        wrappedCommand.Options.Add(option);

                        optionSetters.Add(BuildOptionSetter(subCommandType, propertyOption.PropertyInfo, option));
                    }

                    var handlerType = typeof(ICommandHandler<>).MakeGenericType(subCommandType);
                    var handler = services.GetService(handlerType);
                    if (handler is not null && handlerType.TryGetMethod("Handle", out var methodHandler))
                    {
                        // Pre-compile handler invocation — eliminates MethodInfo.Invoke at parse time
                        var handlerInvoker = BuildHandlerInvoker(handler, methodHandler, subCommandType);

                        wrappedCommand.SetAction(async (pr, ct) =>
                        {
                            foreach (var setter in optionSetters)
                                setter(command, pr);

                            await handlerInvoker(command, cancellationToken);
                        });
                    }
                    else
                    {
                        wrappedCommand.Action = new HelpAction();
                    }

                    commandLineCommand.Subcommands.Add(wrappedCommand);

                    AssignCommandsRecrusive(command, wrappedCommand, services, cancellationToken);
                }
            }
        }
    }

    /// <summary>
    /// Compiles a delegate that reads <paramref name="option"/> from <see cref="ParseResult"/>
    /// and writes it to <paramref name="property"/> on the command instance.
    /// Reflection cost is paid once at registration; parse-time cost is a direct delegate call.
    /// </summary>
    private static Action<ICommand, ParseResult> BuildOptionSetter(
        Type commandType, PropertyInfo property, Option option)
    {
        var valueType = property.PropertyType;
        var typedOptionType = typeof(Option<>).MakeGenericType(valueType);

        var cmdParam = Expression.Parameter(typeof(ICommand), "cmd");
        var prParam = Expression.Parameter(typeof(ParseResult), "pr");

        // (CommandType)cmd
        var castCmd = Expression.Convert(cmdParam, commandType);

        // pr.GetValue<T>((Option<T>)option)  —  option constant already typed as Option<T>
        var getValueCall = Expression.Call(
            prParam,
            _parseResultGetValue.MakeGenericMethod(valueType),
            Expression.Constant(option, typedOptionType));

        // Use GetSetMethod(nonPublic:true) to support internal + init setters
        var setterMethod = property.GetSetMethod(nonPublic: true)
            ?? throw new InvalidOperationException(
                $"Property '{property.Name}' on '{commandType.Name}' has no accessible setter.");

        return Expression.Lambda<Action<ICommand, ParseResult>>(
            Expression.Call(castCmd, setterMethod, getValueCall),
            cmdParam, prParam)
            .Compile();
    }

    /// <summary>
    /// Compiles a delegate that invokes <see cref="ICommandHandler{TCommand}.Handle"/> directly,
    /// bypassing <see cref="MethodInfo.Invoke"/> at parse time.
    /// </summary>
    private static Func<ICommand, CancellationToken, ValueTask> BuildHandlerInvoker(
        object handler, MethodInfo handleMethod, Type commandType)
    {
        var cmdParam = Expression.Parameter(typeof(ICommand), "cmd");
        var ctParam = Expression.Parameter(typeof(CancellationToken), "ct");

        return Expression.Lambda<Func<ICommand, CancellationToken, ValueTask>>(
            Expression.Call(
                Expression.Constant(handler),
                handleMethod,
                Expression.Convert(cmdParam, commandType),
                ctParam),
            cmdParam, ctParam)
            .Compile();
    }

    private static Option CreateOptionInstance(Type typeArg, string name, string[]? aliases)
    {
        Type genericType = typeof(Option<>).MakeGenericType(typeArg);
        ConstructorInfo? ctor = genericType.GetConstructor([typeof(string), typeof(string[])]);
        if (ctor is null)
            throw new InvalidOperationException("Matching constructor not found.");

        return (Option)ctor.Invoke([name, aliases]);
    }
}

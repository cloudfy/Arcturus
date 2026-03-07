using Arcturus.CommandLine.Abstractions;
using Arcturus.CommandLine.Internals;
using Arcturus.Extensions.CommandLine.Internals;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using System.Reflection;

namespace Arcturus.CommandLine;

public static class HostExtensions
{
    public static IHost UseHelp(
        this IHost host, Action<System.CommandLine.Help.HelpContext> helpBuilder, int? maxWidth = null)
    {
        var config = host.Services.GetRequiredService<CommandLineBuilderConfiguration>();
        config.AddConfiguration(builder => builder.UseHelp(helpBuilder, maxWidth));
        return host;
    }

    /// <summary>
    /// Adds a middleware type to the command-line execution pipeline.
    /// </summary>
    /// <typeparam name="TMiddleware">The middleware type.</typeparam>
    /// <param name="host">The host instance.</param>
    /// <returns>The host instance for chaining.</returns>
    public static IHost UseMiddleware<TMiddleware>(this IHost host)
        where TMiddleware : ICommandLineMiddleware
    {
        var config = host.Services.GetRequiredService<CommandLineBuilderConfiguration>();
        config.AddMiddleware(typeof(TMiddleware));
        return host;
    }

    /// <summary>
    /// Run console using commands. <typeparamref name="T"/> serve as root.
    /// </summary>
    /// <typeparam name="T">Type of root command.</typeparam>
    /// <param name="host">Required.</param>
    /// <param name="args">Optional. Parameters from the host.</param>
    /// <returns></returns>
    public static async Task RunConsoleCommands<T>(this IHost host, string[] args)
        where T : ICommand
    {
        // our root cancellation token
        CancellationTokenSource cancellationTokenSource = new();

        var command = (T)ActivatorUtilities.CreateInstance(host.Services, typeof(T));
        var commandDetails = typeof(T).GetCustomAttribute<CommandAttribute>();

        var commandLineCommand = new RootCommand(commandDetails?.Description ?? "CLI");

        CommandLineBuilder commandLineBuilder = new(commandLineCommand);
        commandLineBuilder.UseDefaults();

        // Apply any stored configurations
        var config = host.Services.GetService<CommandLineBuilderConfiguration>();
        config?.Apply(commandLineBuilder);

        HostBuilderExtensions.AssignCommandLineBuilder(commandLineBuilder);

        var parser = commandLineBuilder.Build();
        AssignCommandsRecrusive(command, commandLineCommand, host.Services, cancellationTokenSource.Token);

        // Build and execute the middleware pipeline
        var context = new CommandLineContext
        {
            Services = host.Services,
            Args = args,
            Parser = parser,
            CancellationToken = cancellationTokenSource.Token,
            RootCommand = command
        };

        var pipeline = BuildPipeline(config, async (ctx) =>
        {
            await ctx.Parser.InvokeAsync(ctx.Args);
        });

        await pipeline(context);
    }

    private static void AssignCommandsRecrusive(
        ICommand parentCommand
        , Command commandLineCommand
        , IServiceProvider services
        , CancellationToken cancellationToken)
    {
        var subCommandAttributes = parentCommand.GetType().GetCustomAttributes<SubCommandAttribute>();
        foreach (var subCommandAttribute in subCommandAttributes)
        {
        
            //var subCommandAttribute = parentCommand.GetType().GetCustomAttribute<SubCommandAttribute>();
            if (subCommandAttribute is not null && subCommandAttribute.SubCommands?.Length > 0)
            {
                foreach (var subCommandType in subCommandAttribute.SubCommands.OrderBy(_ => _.Name))
                {
                    var command = (ICommand)Activator.CreateInstance(subCommandType)!;
                    var commandDetails = subCommandType.GetCustomAttribute<CommandAttribute>();
                    if (commandDetails is null)
                        continue;

                    // create the system commandline command (wrapper)
                    var wrappedCommand = new Command(commandDetails.Name, commandDetails.Description);
                    //if (commandDetails.Aliases?.Length > 0)
                    //{
                    //    foreach (var alias in commandDetails.Aliases)
                    //    {
                    //        wrappedCommand.AddAlias(alias);
                    //    }
                    //}

                    foreach (var field in subCommandType
                        .GetProperties()
                        .Select(_ => (attr: _.GetCustomAttribute<OptionAttribute>(), pinfo: _!)).Where(_ => _.attr is not null))
                    {
                        // we use reflection to instanciate
                        var option = CreateOptionInstance(
                            field.pinfo.PropertyType, field.attr!.Name, field.attr?.Description)!;
                        option.IsRequired = !Internals.TypeExtensions.IsNullable(field.pinfo.PropertyType) || 
                            Internals.TypeExtensions.IsRequired(field.pinfo);

                        wrappedCommand.AddOption(option);
                    }

                    // register handler
                    var handlerType = typeof(ICommandHandler<>).MakeGenericType(subCommandType);
                    var handler = services.GetService(handlerType);
                    if (handler is not null && handlerType.TryGetMethod("Handle", out var methodHandler))
                    {
                        wrappedCommand.SetHandler(async (context) =>
                        {
                            //context.ParseResult.GetValueForOption()
                            // context.ParseResult.CommandResult.Children
                            foreach (var o in context.ParseResult.CommandResult.Command.Options)
                            {
                                var ov = context.ParseResult.GetValueForOption(o);

                                var property = subCommandType
                                    .GetProperties()
                                    .Where(_ => _.GetCustomAttribute<OptionAttribute>()?.Name == "--" + o.Name)
                                    .FirstOrDefault();
                                if (property is not null)
                                {
                                    if (Arcturus.CommandLine.Internals.TypeExtensions.IsEnumOrNullableEnum(property.PropertyType))
                                    {
                                        ov = Enum.Parse(
                                            Arcturus.CommandLine.Internals.TypeExtensions.GetEnumType(property.PropertyType)
                                            , ov.ToString(), true);
                                    }
                                    property.SetValue(command, ov);
                                }
                            }
                            await (ValueTask)methodHandler!.Invoke(handler, [command!, cancellationToken])!;
                        });
                    }
                    commandLineCommand.AddCommand(wrappedCommand);

                    AssignCommandsRecrusive(command, wrappedCommand, services, cancellationToken);
                }
            }

        }
    }
    private static Option CreateOptionInstance(Type typeArg, string name, string? description)
    {
        // new Option<string>(field.attr.Name, field.attr.Description);

        // Get the generic type definition
        Type genericType = typeof(Option<>).MakeGenericType(typeArg);

        // Get the constructor that matches (string, string)
        ConstructorInfo ctor = genericType.GetConstructor([typeof(string), typeof(string)]);

        if (ctor == null)
            throw new InvalidOperationException("Matching constructor not found.");

        // Create instance using the constructor
        return (Option)ctor.Invoke([name, description]);
    }

    private static CommandLineDelegate BuildPipeline(
        CommandLineBuilderConfiguration? config,
        CommandLineDelegate finalHandler)
    {
        if (config == null)
            return finalHandler;

        var middlewareTypes = config.GetMiddlewareTypes().Reverse().ToList();

        CommandLineDelegate pipeline = finalHandler;

        foreach (var middlewareType in middlewareTypes)
        {
            var next = pipeline;
            pipeline = context =>
            {
                var middleware = (ICommandLineMiddleware)ActivatorUtilities.CreateInstance(
                    context.Services, middlewareType);
                return middleware.InvokeAsync(context, next);
            };
        }

        return pipeline;
    }
}

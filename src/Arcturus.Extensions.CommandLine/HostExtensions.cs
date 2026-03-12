using Arcturus.CommandLine.Abstractions;
using Arcturus.CommandLine.Internals;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.CommandLine;
using System.CommandLine.Help;
using System.CommandLine.Invocation;

namespace Arcturus.CommandLine;

public static class HostExtensions
{
    /// <summary>
    /// Parses and executes command-line arguments using the specified command-line root type within the given host
    /// context.
    /// </summary>
    /// <remarks>If no arguments are provided, the method displays help information by default. The method
    /// uses the host's services to construct the command-line root and execute the command. This extension method is
    /// typically called from the application's entry point to handle command-line processing.</remarks>
    /// <typeparam name="T">The type of the command-line root. Must inherit from CommandLineRoot.</typeparam>
    /// <param name="host">The host that provides application services and dependency injection for the command-line execution.</param>
    /// <param name="args">The array of command-line arguments to parse and execute. If empty, help information is displayed.</param>
    /// <returns>A task that represents the asynchronous operation of parsing and executing the command-line arguments.</returns>
    public static async Task RunCommandLine<T>(this IHost host, string[] args)
        where T : CommandLineRoot
    {
        // our root cancellation token
        CancellationTokenSource cancellationTokenSource = new();

        var config = host.Services.GetRequiredService<CommandLineConfiguration>();

        // create an instance of the command line root using the host's service provider
        var rootInstance = ActivatorUtilities.CreateInstance<T>(host.Services);

        var commandLineBuilder = new CommandLineBuilder<T>(config);
        var commandLineRoot = commandLineBuilder.Build(host, rootInstance, cancellationTokenSource.Token);

        if (args.Length == 0)
        {
            // if no arguments are provided, show help
            args = ["--help"];
        }

        // The final handler in the pipeline: parse and invoke the command
        CommandLineDelegate finalHandler = async context =>
        {
            var parseResult = commandLineRoot.Parse(context.Args);

            InvocationConfiguration invocationConfiguration = new();
            invocationConfiguration.Output = Console.Out;
            invocationConfiguration.EnableDefaultExceptionHandler = config.EnableDefaultExceptionHandler;

            await parseResult.InvokeAsync(invocationConfiguration, context.CancellationToken);
        };

        // Build the middleware pipeline, wrapping the final command handler
        var pipeline = Pipeline.Build(config.GetMiddlewareTypes(), finalHandler);

        var commandLineContext = new CommandLineContext
        {
            Services = host.Services,
            Args = args,
            CancellationToken = cancellationTokenSource.Token,
            RootCommand = rootInstance
        };
        // execute chain and pipeline
        await pipeline(commandLineContext);
    }

    /// <summary>
    /// Adds the specified middleware type to the command-line processing pipeline for the host.
    /// </summary>
    /// <remarks>This method registers the middleware type so that it will be invoked during command-line
    /// processing. Middleware is executed in the order it is added.</remarks>
    /// <typeparam name="TMiddleware">The type of middleware to add. Must implement the ICommandLineMiddleware interface.</typeparam>
    /// <param name="host">The host to which the middleware will be added.</param>
    /// <returns>The same IHost instance, enabling method chaining.</returns>
    public static IHost UseMiddleware<TMiddleware>(this IHost host)
        where TMiddleware : ICommandLineMiddleware
    {
        var config = host.Services.GetRequiredService<CommandLineConfiguration>();

        config.AddMiddleware(typeof(TMiddleware));
        return host;
    }

    public static IHost UseCommandLineHelp(
        this IHost host, Func<HelpAction, Command, SynchronousCommandLineAction?> configureHelp)
    {
        var config = host.Services.GetRequiredService<CommandLineConfiguration>();
        config.ConfigureHelpDelegate = configureHelp;

        return host;
    }
}
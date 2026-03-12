using Microsoft.Extensions.DependencyInjection;
using System.CommandLine;
using System.CommandLine.Help;
using System.CommandLine.Invocation;

namespace Arcturus.CommandLine;

public class CommandLineConfiguration
{
    private readonly List<Type> _middlewareTypes = [];
    internal Func<HelpAction, Command, SynchronousCommandLineAction?>? ConfigureHelpDelegate;
    internal IEnumerable<Type> GetMiddlewareTypes() => _middlewareTypes;
    internal void AddMiddleware(Type? middleware)
    {
        if (middleware is null) return;

        _middlewareTypes.Add(middleware);
    }
    /// <summary>
    /// Gets or sets the service lifetime used for the handler instance.
    /// </summary>
    /// <remarks>The handler's lifetime determines how long the handler instance is retained by the dependency
    /// injection container. Changing this value affects the handler's reuse and disposal behavior.</remarks>
    public ServiceLifetime HandlerLifeTime { get; set; } = ServiceLifetime.Transient;
    /// <summary>
    /// Gets or sets a value indicating whether the default exception handler is enabled.
    /// </summary>
    /// <remarks>When enabled, unhandled exceptions are automatically caught and processed by the default
    /// handler. Disabling this property may require custom exception handling logic to prevent application
    /// termination.</remarks>
    public bool EnableDefaultExceptionHandler { get; set; } = true;
}

using Arcturus.CommandLine.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Arcturus.CommandLine.Internals;

internal static class Pipeline
{
    /// <summary>
    /// Builds a chained middleware pipeline that terminates with <paramref name="finalHandler"/>.
    /// Middleware is executed in registration order; each component wraps the remainder of the chain.
    /// </summary>
    internal static CommandLineDelegate Build(
        IEnumerable<Type> middlewareTypes,
        CommandLineDelegate finalHandler)
    {
        CommandLineDelegate pipeline = finalHandler;

        foreach (var middlewareType in middlewareTypes.Reverse())
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

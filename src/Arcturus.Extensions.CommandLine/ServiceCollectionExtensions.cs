using Arcturus.CommandLine.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Arcturus.CommandLine;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds command-line configuration and command handler registration to the specified service collection.
    /// </summary>
    /// <remarks>This method registers the command-line configuration as a singleton and automatically
    /// registers command handlers based on the specified handler lifetime. Call this method during application startup
    /// to enable command-line processing.</remarks>
    /// <param name="services">The service collection to which the command-line configuration and handlers will be added. Cannot be null.</param>
    /// <param name="builder">An optional delegate to configure the command-line options before registration. If null, default configuration
    /// is used.</param>
    /// <returns>The same instance of the service collection for chaining further registrations.</returns>
    public static IServiceCollection AddCommandLine(
        this IServiceCollection services
        , Action<CommandLineConfiguration>? builder = null)
    {
        var configuration = new CommandLineConfiguration();
        builder?.Invoke(configuration);

        services.AddSingleton(configuration);

        AutoRegisterCommandHandlers(services, configuration.HandlerLifeTime);
        return services;
    }
    
    private static IServiceCollection AutoRegisterCommandHandlers(
        this IServiceCollection services, ServiceLifetime serviceLifetime)
    {
        var handlerTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => t.GetInterfaces().Any(i =>
                i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommandHandler<>)))
            .ToList();

        foreach (var handlerType in handlerTypes)
        {
            var commandInterface = handlerType.GetInterfaces()
                .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommandHandler<>));

            services.Add(new ServiceDescriptor(commandInterface, handlerType, serviceLifetime));
        }

        return services;
    }
}

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Arcturus.EventBus;

public static class ServiceExtensions
{
    /// <summary>
    /// Adds EventBus services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">Configuration action for the EventBus builder.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddEventBus(
        this IServiceCollection services
        , Action<EventBusBuilder> configure)
    {
        var builder = new EventBusBuilder(services);

        // Apply user configuration
        configure(builder);

        // Register core EventBus services
        RegisterCoreServices(services, builder);

        return services;
    }

    private static void RegisterCoreServices(IServiceCollection services, EventBusBuilder builder)
    {        
        // Register EventBusOptions
        var options = builder.BuildOptions();
        services.TryAddSingleton(options);

        // registry
        services.TryAddSingleton<EventTypeRegistry>((sp) => new EventTypeRegistry(
            builder.ScanAllAssemblies 
                ? Internals.AssemblyExtensions.LoadAllAssemblies()
                : builder.AssembliesToScan));

        // serialization
        services.TryAddSingleton<Serialization.DefaultEventMessageTypeResolver>();
        services.TryAddSingleton<Serialization.IEventMessageSerializer, Serialization.DefaultEventMessageSerializer>();

    }
}
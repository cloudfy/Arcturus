using Arcturus.Mediation.Abstracts;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Arcturus.Mediation;

/// <summary>
/// Extension methods for configuring mediation services in the DI container.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds mediation services to the specified service collection.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="configureAction">An optional action to configure the mediation services.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddMediation(
        this IServiceCollection services,
        Action<MediationConfiguration>? configureAction = null)
    {
        var configuration = new MediationConfiguration();
        configureAction?.Invoke(configuration);

        // Register the mediator
        services.AddScoped<IMediator, Mediator>();

        // Register handlers from assemblies
        foreach (var assembly in configuration.Assemblies)
        {
            RegisterHandlersFromAssembly(services, assembly);
        }

        // Register middleware
        foreach (var middlewareType in configuration.MiddlewareTypes)
        {
            services.AddScoped(typeof(IMiddleware), middlewareType);
        }

        return services;
    }

    /// <summary>
    /// Adds mediation services and automatically registers handlers from the calling assembly.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddMediationFromCallingAssembly(this IServiceCollection services)
    {
        var callingAssembly = Assembly.GetCallingAssembly();
        return services.AddMediation(config => config.RegisterHandlersFromAssembly(callingAssembly));
    }

    private static void RegisterHandlersFromAssembly(IServiceCollection services, Assembly assembly)
    {
        var types = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract)
            .ToList();

        // Register request handlers with response
        foreach (var type in types)
        {
            var handlerInterfaces = type.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>))
                .ToList();

            foreach (var handlerInterface in handlerInterfaces)
            {
                services.AddScoped(handlerInterface, type);
            }
        }

        // Register request handlers without response
        foreach (var type in types)
        {
            var handlerInterfaces = type.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequestHandler<>))
                .ToList();

            foreach (var handlerInterface in handlerInterfaces)
            {
                services.AddScoped(handlerInterface, type);
            }
        }

        // Register notification handlers
        foreach (var type in types)
        {
            var handlerInterfaces = type.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(INotificationHandler<>))
                .ToList();

            foreach (var handlerInterface in handlerInterfaces)
            {
                services.AddScoped(handlerInterface, type);
            }
        }
    }
}

using Arcturus.Mediation.Abstracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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
        services.Add(new ServiceDescriptor(typeof(IMediator), typeof(Mediator), configuration.LifeTime));
        services.AddSingleton<MediationConfiguration>(configuration);

        // Register handlers from assemblies
        foreach (var assembly in configuration.Assemblies)
        {
            RegisterHandlersFromAssembly(services, assembly, configuration.LifeTime);
        }

        // Register middleware
        foreach (var middlewareType in configuration.MiddlewareTypes)
        {
            services.Add(new ServiceDescriptor(typeof(IMiddleware), middlewareType, configuration.LifeTime));
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

    // run during startup to register handlers from a specific assembly, no need to cache
    private static void RegisterHandlersFromAssembly(
        IServiceCollection services, Assembly assembly, ServiceLifetime lifeTime)
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
                services.Add(new ServiceDescriptor(handlerInterface, type, lifeTime));
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
                services.Add(new ServiceDescriptor(handlerInterface, type, lifeTime));
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
                services.Add(new ServiceDescriptor(handlerInterface, type, lifeTime));
            }
        }
    }
}

using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Reflection;

namespace Arcturus.AspNetCore.Endpoints;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds all classes that implement IEndPointModule to the service collection.
    /// <para>
    /// Remember to call <see cref="MapEndpointModules(IEndpointRouteBuilder)"/>.
    /// </para>
    /// </summary>
    /// <param name="services">Required.</param>
    /// <param name="configuration">Optional. </param>
    /// <returns><see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddEndpointModules(this IServiceCollection services, Action<EndpointModuleConfiguration>? configuration = null) 
        => AddEndpointModules(services, Assembly.GetCallingAssembly(), configuration);

    /// <summary>
    /// Adds all classes that implement IEndPointModule to the service collection.
    /// <para>
    /// Remember to call <see cref="MapEndpointModules(IEndpointRouteBuilder)"/>.
    /// </para>
    /// </summary>
    /// <param name="services">Required.</param>
    /// <param name="assembly"><see cref="Assembly"/> to get array of <see cref="IEndPointModule"/> from.</param>
    /// <param name="configuration">Optional. </param>
    /// <returns><see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddEndpointModules(
        this IServiceCollection services
        , Assembly assembly
        , Action<EndpointModuleConfiguration>? configuration = null)
    {
        var config = new EndpointModuleConfiguration();
        configuration?.Invoke(config);

        services.AddSingleton<EndpointModuleConfiguration>(config);

        var modules = assembly
            .GetTypes()
            .Where(
                t => !t.IsAbstract
                && typeof(IEndPointModule).IsAssignableFrom(t)
                && t != typeof(IEndPointModule)
                && (t.IsPublic || t.IsNestedPublic)
            );
        foreach (var module in modules)
        {
            if (config.Lifetime == ServiceLifetime.Scoped)
            {
                services.AddScoped(typeof(IEndPointModule), module);
            }
            else if (config.Lifetime == ServiceLifetime.Transient)
            {
                services.AddTransient(typeof(IEndPointModule), module);
            }
            else // Default to Singleton
            {
                services.AddSingleton(typeof(IEndPointModule), module);
            }
        }

        return services;
    }
    /// <summary>
    /// Maps all endpoint modules to the endpoint route builder.
    /// <para>
    /// Call <see cref="AddEndpointModules(IServiceCollection)"/> before calling this method."/>
    /// </para>
    /// </summary>
    /// <param name="builder">Required.</param>
    /// <returns><see cref="IEndpointRouteBuilder"/></returns>
    public static IEndpointRouteBuilder MapEndpointModules(this IEndpointRouteBuilder builder)
    {
        var config = builder.ServiceProvider.GetRequiredService<EndpointModuleConfiguration>();
        var logger = builder.ServiceProvider.GetService<ILogger<IEndpointRouteBuilder>>();

        if (config.Lifetime == ServiceLifetime.Scoped)
        {
            logger?.LogTrace("Registration using scope");
            // Create a scope to resolve scoped services
            using (var scope = builder.ServiceProvider.CreateScope())
            {
                var modules = scope.ServiceProvider.GetServices<IEndPointModule>();
                foreach (var module in modules)
                {
                    try
                    {
                        module.AddRoute(builder);
                    }
                    catch (NotImplementedException)
                    {
                        // let it go
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
        }
        else
        {
            logger?.LogTrace("Registration using non-scope");
            foreach (var moduleInterface in builder.ServiceProvider.GetServices<IEndPointModule>())
            {
                moduleInterface.AddRoute(builder);
            }
        }

        return builder;
    }
}

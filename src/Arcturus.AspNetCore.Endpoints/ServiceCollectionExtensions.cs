using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
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
    /// <returns><see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddEndpointModules(this IServiceCollection services) 
        => AddEndpointModules(services, Assembly.GetCallingAssembly());

    /// <summary>
    /// Adds all classes that implement IEndPointModule to the service collection.
    /// <para>
    /// Remember to call <see cref="MapEndpointModules(IEndpointRouteBuilder)"/>.
    /// </para>
    /// </summary>
    /// <param name="services">Required.</param>
    /// <param name="assembly"><see cref="Assembly"/> to get array of <see cref="IEndPointModule"/> from.</param>
    /// <returns><see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddEndpointModules(this IServiceCollection services, Assembly assembly)
    {
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
            services.AddSingleton(typeof(IEndPointModule), module);
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
        foreach (var moduleInterface in builder.ServiceProvider.GetServices<IEndPointModule>())
        {
            moduleInterface.AddRoute(builder);
        }

        return builder;
    }
}

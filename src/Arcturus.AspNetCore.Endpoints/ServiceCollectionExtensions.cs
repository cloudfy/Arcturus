using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
    /// Call <see cref="AddEndpointModules(IServiceCollection, Action{EndpointModuleConfiguration}?)" /> before calling this method.
    /// </para>
    /// </summary>
    /// <param name="builder">Required.</param>
    /// <returns><see cref="IEndpointRouteBuilder"/></returns>
    public static IEndpointRouteBuilder UseEndpointModules(this IEndpointRouteBuilder builder)
        => UseEndpointModules(builder, null);

    /// <summary>
    /// Maps all endpoint modules to the endpoint route builder with optional global endpoint conventions.
    /// <para>
    /// Call <see cref="AddEndpointModules(IServiceCollection, Action{EndpointModuleConfiguration}?)" /> before calling this method.
    /// </para>
    /// <para>
    /// The <paramref name="configure"/> delegate allows you to apply common endpoint conventions (such as <c>.RequireAuthorization()</c>, <c>.AllowAnonymous()</c>, <c>.WithMetadata(...)</c>) to endpoints registered by modules.
    /// Modules implementing <see cref="IConfigurableEndPointModule"/> receive this delegate and can invoke it for each endpoint they register.
    /// Legacy modules that only implement <see cref="IEndPointModule"/> do not receive this delegate.
    /// </summary>
    /// <param name="builder">Required.</param>
    /// <param name="configure">Optional. A delegate to configure endpoint conventions that will be applied to all endpoints registered by each module.</param>
    /// <returns><see cref="IEndpointRouteBuilder"/></returns>
    /// <example>
    /// <code>
    /// app.UseEndpointModules(endpoint => endpoint.RequireAuthorization());
    /// </code>
    /// </example>
    public static IEndpointRouteBuilder UseEndpointModules(
        this IEndpointRouteBuilder builder
        , Action<IEndpointConventionBuilder>? configure)
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
                    // Check if the module implements IConfigurableEndPointModule
                    if (module is IConfigurableEndPointModule configurableModule)
                    {
                        configurableModule.AddRoute(builder, configure);
                    }
                    else
                    {
                        // Fall back to the base AddRoute method for legacy modules
                        module.AddRoute(builder);
                    }
                }
            }
        }
        else
        {
            logger?.LogTrace("Registration using non-scope");
            foreach (var moduleInterface in builder.ServiceProvider.GetServices<IEndPointModule>())
            {
                // Check if the module implements IConfigurableEndPointModule
                if (moduleInterface is IConfigurableEndPointModule configurableModule)
                {
                    configurableModule.AddRoute(builder, configure);
                }
                else
                {
                    // Fall back to the base AddRoute method for legacy modules
                    moduleInterface.AddRoute(builder);
                }
            }
        }

        return builder;
    }

    /// <summary>
    /// Maps all endpoint modules to the endpoint route builder.
    /// <para>
    /// Call <see cref="AddEndpointModules(IServiceCollection, Action{EndpointModuleConfiguration}?)" /> before calling this method.
    /// </para>
    /// </summary>
    /// <param name="builder">Required.</param>
    /// <returns><see cref="IEndpointRouteBuilder"/></returns>
    [Obsolete("Use UseEndpointModules instead. This method is just an alias for UseEndpointModules and will be removed in future versions.")]
    public static IEndpointRouteBuilder MapEndpointModules(this IEndpointRouteBuilder builder)
        => UseEndpointModules(builder);

    /// <summary>
    /// Maps all endpoint modules to the endpoint route builder with optional global endpoint conventions.
    /// <para>
    /// Call <see cref="AddEndpointModules(IServiceCollection, Action{EndpointModuleConfiguration}?)" /> before calling this method.
    /// </para>
    /// </summary>
    /// <param name="builder">Required.</param>
    /// <param name="configure">Optional. A delegate to configure endpoint conventions that will be applied to all endpoints registered by each module.</param>
    /// <returns><see cref="IEndpointRouteBuilder"/></returns>
    [Obsolete("Use UseEndpointModules instead. This method is just an alias for UseEndpointModules and will be removed in future versions.")]
    public static IEndpointRouteBuilder MapEndpointModules(this IEndpointRouteBuilder builder, Action<IEndpointConventionBuilder>? configure)
        => UseEndpointModules(builder, configure);
}

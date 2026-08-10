using Microsoft.Extensions.DependencyInjection;

namespace Arcturus.AspNetCore.Endpoints;

/// <summary>
/// Configuration options for endpoint modules.
/// <para>
/// Use <see cref="ServiceCollectionExtensions.AddEndpointModules(IServiceCollection, Action{EndpointModuleConfiguration}?)"/> to configure these options.
/// </para>
/// </summary>
public class EndpointModuleConfiguration
{
    /// <summary>
    /// Gets or sets the lifetime of the endpoint modules.
    /// <para>
    /// Default is <see cref="ServiceLifetime.Scoped" />.
    /// </para>
    /// </summary>
    public ServiceLifetime Lifetime = ServiceLifetime.Scoped;
}

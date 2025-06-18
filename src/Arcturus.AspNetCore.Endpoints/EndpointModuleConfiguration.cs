using Microsoft.Extensions.DependencyInjection;

namespace Arcturus.AspNetCore.Endpoints;

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

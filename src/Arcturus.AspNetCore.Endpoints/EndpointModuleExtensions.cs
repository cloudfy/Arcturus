using Microsoft.AspNetCore.Builder;

namespace Arcturus.AspNetCore.Endpoints;

public static class EndpointModuleExtensions
{
    /// <summary>
    /// Applies the provided configuration action to the <see cref="IEndpointConventionBuilder"/>.
    /// </summary>
    /// <param name="app"></param>
    /// <param name="configure">Global configuration action to apply to the endpoint.</param>
    /// <returns><see cref="IEndpointConventionBuilder"/></returns>
    public static IEndpointConventionBuilder ApplyEndpointConfiguration(
        this IEndpointConventionBuilder app
        , Action<IEndpointConventionBuilder>? configure)
    {
        configure?.Invoke(app);
        return app;
    }
}

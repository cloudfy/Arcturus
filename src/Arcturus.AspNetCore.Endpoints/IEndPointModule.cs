using Microsoft.AspNetCore.Routing;

namespace Arcturus.AspNetCore.Endpoints;

/// <summary>
/// Defines an endpoint module.
/// </summary>
public interface IEndPointModule
{
    /// <summary>
    /// Adds an <see cref="IEndPointModule"/> route to the <see cref="IEndpointRouteBuilder" />.
    /// </summary>
    /// <param name="app">Required. The endpoint route builder.</param>
    void AddRoute(IEndpointRouteBuilder app);
}
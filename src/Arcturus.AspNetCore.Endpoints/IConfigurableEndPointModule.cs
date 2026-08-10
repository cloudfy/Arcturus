using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Arcturus.AspNetCore.Endpoints;

/// <summary>
/// Defines an endpoint module that supports global endpoint conventions.
/// <para>
/// Implement this interface instead of <see cref="IEndPointModule"/> when your module needs to apply
/// global conventions (such as <c>.RequireAuthorization()</c>, <c>.AllowAnonymous()</c>, <c>.WithMetadata(...)</c>, etc.)
/// to endpoints registered by the module.
/// </para>
/// </summary>
public interface IConfigurableEndPointModule : IEndPointModule
{
    /// <summary>
    /// Adds an <see cref="IEndPointModule"/> route to the <see cref="IEndpointRouteBuilder" /> with optional global endpoint conventions.
    /// <para>
    /// The <paramref name="configure"/> delegate can be used to apply common conventions (like <c>.RequireAuthorization()</c>, <c>.AllowAnonymous()</c>, <c>.WithMetadata(...)</c>, etc.) to all endpoints registered by this module.
    /// </para>
    /// </summary>
    /// <param name="app">Required. The endpoint route builder.</param>
    /// <param name="configure">Optional. A delegate to configure endpoint conventions that will be applied to all endpoints registered by this module.</param>
    /// <example>
    /// <code>
    /// public void AddRoute(IEndpointRouteBuilder app, Action&lt;IEndpointConventionBuilder&gt;? configure = null)
    /// {
    ///     var endpoint = app.MapGet("/api/users", () => "Users");
    ///     configure?.Invoke(endpoint);
    /// }
    /// </code>
    /// </example>
    void AddRoute(IEndpointRouteBuilder app, Action<IEndpointConventionBuilder>? configure = null);
}
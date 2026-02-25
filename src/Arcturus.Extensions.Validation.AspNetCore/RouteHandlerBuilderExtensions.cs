using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Arcturus.Validation;

public static class RouteHandlerBuilderExtensions
{
    /// <summary>
    /// Adds a parameter validation filter to the specified route handler builder.
    /// </summary>
    /// <remarks>Use this method to ensure that parameters for the associated route handler are validated
    /// before the handler executes. This is typically called during route configuration to enforce validation rules
    /// automatically.</remarks>
    /// <typeparam name="TBuilder">The type of the route handler builder being extended.</typeparam>
    /// <param name="builder">The route handler builder to which the parameter validation filter is applied.</param>
    /// <returns>The route handler builder instance with parameter validation enabled.</returns>
    public static RouteHandlerBuilder ValidateParameters<TBuilder>(this RouteHandlerBuilder builder)         
    {
        builder.AddEndpointFilter<ValidateParametersFilter>();
        return builder;
    }
}
using Arcturus.Repository.Json.Internals;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

namespace Arcturus.Extensions.Repository.Json;

/// <summary>
/// Provides extension methods for configuring JSON serialization options for Entity Framework in the service collection.
/// </summary>
public static class ServicesExtensions
{
    /// <summary>
    /// Configures JSON serialization options for Entity Framework in the service collection.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="options">An action to configure the JSON serializer options.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection ConfigureEFCorePropertyJsonOptions(
        this IServiceCollection services
        , Action<JsonSerializerOptions> options)
    {
        SpecificEfJsonSerializer.ConfigureJsonOptions(options);
        return services;
    }

    [Obsolete("Use ConfigureEFCorePropertyJsonOptions instead.")]
    public static IServiceCollection ConfigureEfJsonOptions(
        this IServiceCollection services
        , Action<JsonSerializerOptions> options)
        => ConfigureEFCorePropertyJsonOptions(services, options);
}

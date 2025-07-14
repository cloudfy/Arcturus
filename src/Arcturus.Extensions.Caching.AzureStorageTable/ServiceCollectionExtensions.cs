using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Internal;

namespace Arcturus.Extensions.Caching.AzureStorageTable;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Azure Storage Table-based distributed caching to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <remarks>This method registers the necessary services for using Azure Storage Table as a distributed
    /// cache. It configures the cache options using the provided <paramref name="configureOptions"/> delegate and
    /// ensures that required dependencies, such as <see cref="ISystemClock"/> and <see cref="IDistributedCache"/>, are
    /// added to the service collection.</remarks>
    /// <param name="services">The <see cref="IServiceCollection"/> to which the caching services will be added.</param>
    /// <param name="configureOptions">A delegate to configure the <see cref="AzureStorageTableCacheOptions"/> used to set up the Azure Storage Table
    /// cache.</param>
    /// <returns>The same <see cref="IServiceCollection"/> instance so that additional calls can be chained.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="services"/> or <paramref name="configureOptions"/> is <see langword="null"/>.</exception>
    public static IServiceCollection AddAzureStorageTableCache(
        this IServiceCollection services,
        Action<AzureStorageTableCacheOptions> configureOptions)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (configureOptions == null) throw new ArgumentNullException(nameof(configureOptions));

        var options = new AzureStorageTableCacheOptions();
        configureOptions(options);
        services.AddSingleton(options);
        services.TryAddSingleton<ISystemClock, SystemClock>();
        services.AddSingleton<IDistributedCache, AzureStorageTableCache>();
        return services;
    }
}

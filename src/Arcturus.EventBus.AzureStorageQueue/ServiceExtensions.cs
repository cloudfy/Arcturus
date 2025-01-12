using Arcturus.EventBus.Abstracts;
using Microsoft.Extensions.DependencyInjection;

namespace Arcturus.EventBus.AzureStorageQueue;

public static class ServiceExtensions
{
    /// <summary>
    /// Adds Arcturus.EventBus implementation using Azure Storage Queues to the service collection.
    /// </summary>
    /// <param name="services">Required. Service collection.</param>
    /// <param name="options">Optional. Options to configure.</param>
    /// <returns></returns>
    public static IServiceCollection AddAzureStorageQueueEventBus(
        this IServiceCollection services
        , Action<StorageQueueOptions>? options = null)
    {
        var currentOptions = new StorageQueueOptions();
        if (options is not null)
        {
            options(currentOptions);
        }

        services.AddSingleton<IConnection, StorageQueueConnection>(
            (sp) => { return new StorageQueueConnection(currentOptions); });
        services.AddSingleton<IEventBusFactory, StorageQueueFactory>();
        services.AddSingleton<StorageQueueOptions>(currentOptions);

        return services;
    }
}

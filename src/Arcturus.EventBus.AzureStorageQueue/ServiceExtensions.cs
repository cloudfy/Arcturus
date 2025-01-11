using Arcturus.EventBus.Abstracts;
using Microsoft.Extensions.DependencyInjection;

namespace Arcturus.EventBus.AzureStorageQueue;

public static class ServiceExtensions
{
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

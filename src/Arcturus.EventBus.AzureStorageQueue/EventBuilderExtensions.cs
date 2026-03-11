using Arcturus.EventBus.Abstracts;
using Microsoft.Extensions.DependencyInjection;

namespace Arcturus.EventBus.AzureStorageQueue;

public static class EventBuilderExtensions
{
    /// <summary>
    /// Adds Azure Storage Queue as the event bus provider.
    /// </summary>
    /// <param name="builder">The event bus builder to which the Azure Storage Queue services will be added.</param>
    /// <param name="options">An optional delegate to configure the storage queue options for the event bus.</param>
    /// <returns>The event bus builder instance with Azure Storage Queue services registered.</returns>
    public static EventBusBuilder AddAzureStorageQueueEventBus(
        this EventBusBuilder builder
        , Action<StorageQueueOptions>? options = null)
    {
        var currentOptions = new StorageQueueOptions();
        if (options is not null)
        {
            options(currentOptions);
        }

        currentOptions.ApplicationId = builder.ApplicationId;
        currentOptions.UseEventHandlersProcessor = builder.UseEventHandlersProcessor;

        // add services
        builder.Services.AddSingleton<IConnection, StorageQueueConnection>(
            (sp) => { return new StorageQueueConnection(currentOptions); });
        builder.Services.AddSingleton<IEventBusFactory, StorageQueueFactory>();
        builder.Services.AddSingleton<StorageQueueOptions>(currentOptions);

        return builder;
    }
}
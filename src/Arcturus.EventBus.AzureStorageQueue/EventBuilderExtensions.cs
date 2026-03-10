using Arcturus.EventBus.Abstracts;
using Microsoft.Extensions.DependencyInjection;

namespace Arcturus.EventBus.AzureStorageQueue;

public static class EventBuilderExtensions
{
    /// <summary>
    /// Adds SQLite-based event bus services to the specified event bus builder.
    /// </summary>
    /// <remarks>This method registers the necessary services for using an SQLite-backed event bus, including
    /// connection and factory services. Ensure that the storage queue options are configured to meet the application's
    /// requirements.</remarks>
    /// <param name="builder">The event bus builder to which the SQLite event bus services will be added.</param>
    /// <param name="options">An optional delegate to configure the storage queue options for the event bus. If provided, this action allows
    /// customization of the storage queue settings.</param>
    /// <returns>The event bus builder instance with SQLite event bus services registered.</returns>
    
    public static EventBusBuilder AddSqliteEventBus(
        this EventBusBuilder builder
        , Action<StorageQueueOptions>? options = null)
    {
        var currentOptions = new StorageQueueOptions();
        if (options is not null)
        {
            options(currentOptions);
        }

        // add services
        builder.Services.AddSingleton<IConnection, StorageQueueConnection>(
            (sp) => { return new StorageQueueConnection(currentOptions); });
        builder.Services.AddSingleton<IEventBusFactory, StorageQueueFactory>();
        builder.Services.AddSingleton<StorageQueueOptions>(currentOptions);

        return builder;
    }
}
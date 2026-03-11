using Arcturus.EventBus.Abstracts;
using Arcturus.EventBus.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Arcturus.EventBus.AzureStorageQueue;

public class StorageQueueFactory(
    IConnection connection
    , IServiceProvider serviceProvider
    , ILoggerFactory loggerFactory
    , StorageQueueOptions eventBusOptions) : IEventBusFactory
{
    private readonly IConnection _connection = connection;
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly StorageQueueOptions _eventBusOptions = eventBusOptions;

    public IProcessor CreateProcessor(string? queue = null)
    {
        var eventMessageSerializer = _serviceProvider.GetRequiredService<IEventMessageSerializer>();

        // returns a wrapper processor that will handle the event handlers
        // and fallback to the processor if no handlers are found
        if (_eventBusOptions.UseEventHandlersProcessor)
            return new EventHandlersProcessor(
                new StorageQueueProcessor(
                    _connection
                    , _eventBusOptions
                    , eventMessageSerializer
                    , queue ?? _eventBusOptions.DefaultQueueName), _serviceProvider, loggerFactory);

        return new StorageQueueProcessor(
            _connection, _eventBusOptions, eventMessageSerializer, queue ?? _eventBusOptions.DefaultQueueName);
    }

    public IPublisher CreatePublisher(string? queue = null)
    {
        var eventMessageSerializer = _serviceProvider.GetRequiredService<IEventMessageSerializer>();
        
        return new StorageQueuePublisher(
            _connection, queue ?? _eventBusOptions.DefaultQueueName, loggerFactory, eventMessageSerializer);
    }
}

using Arcturus.EventBus.Abstracts;
using Arcturus.EventBus.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Arcturus.EventBus.RabbitMQ;

public sealed class RabbitMQEventBusFactory(
    IConnection connection
    , IServiceProvider serviceProvider
    , ILoggerFactory loggerFactory
    , RabbitMQEventBusOptions eventBusOptions) : IEventBusFactory
{
    private readonly IConnection _connection = connection;
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly RabbitMQEventBusOptions _eventBusOptions = eventBusOptions;

    public IPublisher CreatePublisher(string? queue = null)
    {
        var eventMessageSerializer = _serviceProvider.GetRequiredService<IEventMessageSerializer>();

        return new RabbitMQPublisher(_connection, loggerFactory, eventMessageSerializer, queue ?? _eventBusOptions.DefaultQueueName);
    }
    public IProcessor CreateProcessor(string? queue = null)
    {
        var eventMessageSerializer = _serviceProvider.GetRequiredService<IEventMessageSerializer>();

        // returns a wrapper processor that will handle the event handlers
        // and fallback to the processor if no handlers are found
        if (_eventBusOptions.UseEventHandlersProcessor.GetValueOrDefault(true))
            return new EventHandlersProcessor(
                new RabbitMQProcessor(_connection, loggerFactory, eventMessageSerializer, queue ?? _eventBusOptions.DefaultQueueName), _serviceProvider, loggerFactory);

        return new RabbitMQProcessor(_connection, loggerFactory, eventMessageSerializer, queue ?? _eventBusOptions.DefaultQueueName);
    }
}

using Arcturus.EventBus.Abstracts;
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
        return new RabbitMQPublisher(_connection, loggerFactory, queue);
    }
    public IProcessor CreateProcessor(string? queue = null)
    {
        // returns a wrapper
        if (_eventBusOptions.UseEventHandlersProcessor.GetValueOrDefault(true))
            return new EventHandlersProcessor(
                new RabbitMQProcessor(_connection, queue), _serviceProvider, loggerFactory);
        return new RabbitMQProcessor(connection, queue);
    }
    //public ISubscriber CreateSubscriber()
    //{
    //    throw new NotImplementedException();
    //}
}

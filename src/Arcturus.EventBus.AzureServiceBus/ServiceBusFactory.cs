using Arcturus.EventBus.Abstracts;
using Microsoft.Extensions.Logging;

namespace Arcturus.EventBus.AzureServiceBus;
public sealed class ServiceBusFactory(
    IConnection connection
    , IServiceProvider serviceProvider
    , ILoggerFactory loggerFactory
    , ServiceBusOptions options) : IEventBusFactory
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly IConnection _connection = connection;
    private readonly ILoggerFactory _loggerFactory = loggerFactory;
    private readonly ServiceBusOptions _options = options;

    public IProcessor CreateProcessor(string? queue = null)
    {
        // returns a wrapper processor that will handle the event handlers
        // and fallback to the processor if no handlers are found
        if (_options.UseEventHandlersProcessor.GetValueOrDefault(true))
            return new EventHandlersProcessor(
                new ServiceBusProcessor(
                    _connection
                    , _options
                    , queue ?? _options.DefaultQueueName, _loggerFactory)
                , _serviceProvider, _loggerFactory);

        return new ServiceBusProcessor(_connection, _options, queue, _loggerFactory);
    }

    public IPublisher CreatePublisher(string? queue = null)
    {
        return new ServiceBusPublisher(_connection, queue, _options, _loggerFactory);
    }
}

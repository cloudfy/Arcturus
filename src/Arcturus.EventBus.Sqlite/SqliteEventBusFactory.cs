using Arcturus.EventBus.Abstracts;
using Microsoft.Extensions.Logging;

namespace Arcturus.EventBus.Sqlite;

public sealed class SqliteEventBusFactory(
    IConnection connection
    , IServiceProvider serviceProvider
    , ILoggerFactory loggerFactory
    , SqliteEventBusOptions eventBusOptions) : IEventBusFactory
{
    private readonly IConnection _connection = connection;
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly SqliteEventBusOptions _eventBusOptions = eventBusOptions;

    public IProcessor CreateProcessor(string? queue = null)
    {
        // returns a wrapper processor that will handle the event handlers
        // and fallback to the processor if no handlers are found
        if (_eventBusOptions.UseEventHandlersProcessor.GetValueOrDefault(true))
            return new EventHandlersProcessor(
                new SqliteProcessor(_connection, queue ?? _eventBusOptions.DefaultQueueName, _eventBusOptions.ProcessInterval.GetValueOrDefault(100))
                , _serviceProvider
                , loggerFactory);

        return new SqliteProcessor(_connection, queue ?? _eventBusOptions.DefaultQueueName, _eventBusOptions.ProcessInterval.GetValueOrDefault(100));
    }

    public IPublisher CreatePublisher(string? queue = null)
    {
        return new SqlitePublisher(_connection, loggerFactory, queue ?? _eventBusOptions.DefaultQueueName);
    }
}
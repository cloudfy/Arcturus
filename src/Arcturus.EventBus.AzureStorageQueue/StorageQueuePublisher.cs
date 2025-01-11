using Arcturus.EventBus.Abstracts;
using Arcturus.EventBus.AzureStorageQueue.Internals;
using Microsoft.Extensions.Logging;

namespace Arcturus.EventBus.AzureStorageQueue;

public sealed class StorageQueuePublisher : IPublisher
{
    private readonly string _queue;
    private readonly StorageQueueConnection _connection;
    private readonly ILogger<StorageQueuePublisher> _logger;

    internal StorageQueuePublisher(
        IConnection connection
        , string? queue
        , ILoggerFactory loggerFactory)
    {
        _queue = queue ?? "default_queue";
        _connection = (connection as StorageQueueConnection)!;
        _logger = loggerFactory.CreateLogger<StorageQueuePublisher>();
    }

    public async Task Publish<TEvent>(TEvent @event, CancellationToken cancellationToken = default) 
        where TEvent : IEventMessage
    {
        var queueClient = await _connection.GetQueueClient(_queue);

        _logger.LogTrace("Publishing event {EventName} to queue {QueueName}", @event.GetType().Name, _queue);

        await queueClient.SendMessageAsync(
            EventMessageSerializer.Serialize(@event)
            , null
            , null
            , cancellationToken);
    }
}
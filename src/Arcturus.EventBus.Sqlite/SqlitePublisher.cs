using Arcturus.EventBus.Abstracts;
using Microsoft.Extensions.Logging;

namespace Arcturus.EventBus.Sqlite;

public sealed class SqlitePublisher : IPublisher
{
    private readonly SqliteConnection _connection;
    private readonly string _queueName;
    private readonly ILogger<SqlitePublisher> _logger;

    internal SqlitePublisher(IConnection connection, ILoggerFactory loggerFactory, string? queueName)
    {
        if (connection is not SqliteConnection)
            throw new NotImplementedException($"Requires SqliteConnection");

        _connection = (SqliteConnection)connection;
        _queueName = queueName ?? "default_queue";

        _logger = loggerFactory.CreateLogger<SqlitePublisher>();
    }

    public async Task Publish<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : IEventMessage
    {
        _logger.LogDebug(
            "Publishing event of type {EventType} to queue {QueueName}"
            , @event.GetType().Name, _queueName);
        await _connection.QueueEvent(@event, _queueName, cancellationToken);
    }
}

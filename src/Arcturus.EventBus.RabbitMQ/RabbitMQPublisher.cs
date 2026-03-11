using Arcturus.EventBus.Abstracts;
using Arcturus.EventBus.Diagnostics;
using Arcturus.EventBus.Serialization;
using Microsoft.Extensions.Logging;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System.Net.Sockets;
using System.Text;

namespace Arcturus.EventBus.RabbitMQ;

public sealed class RabbitMQPublisher : IPublisher, IDisposable
{
    private readonly RabbitMQConnection _connection;
    private readonly string _queueName;
    private readonly IEventMessageSerializer _eventMessageSerializer;
    private IChannel? _channel;
    private readonly ILogger<RabbitMQPublisher> _logger;

    internal RabbitMQPublisher(
        Abstracts.IConnection connection
        , ILoggerFactory loggerFactory
        , IEventMessageSerializer eventMessageSerializer
        , string? queueName = null)
    {
        if (connection is not RabbitMQConnection)
            throw new NotImplementedException($"Requires RabbitMQConnection");

        _connection = (RabbitMQConnection)connection;
        _queueName = queueName ?? "default_queue";
        _eventMessageSerializer = eventMessageSerializer;
        _logger = loggerFactory.CreateLogger<RabbitMQPublisher>();
    }

    public async Task Publish<TEvent>(
        TEvent @event
        , CancellationToken cancellationToken = default) where TEvent : IEventMessage
    {
        // retry policy
        var policy = Policy
            .Handle<AlreadyClosedException>()
            .Or<SocketException>()
            .Or<IOException>()
            .WaitAndRetry(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (exception, timeSpan) =>
            {
                _channel?.Dispose();
                _channel = null;

                _logger.LogDebug(exception, "Could not publish event #{EventId} after {Timeout} seconds: {ExceptionMessage}.", @event, $"{timeSpan.TotalSeconds:n1}", exception.Message);
            });

        await policy.Execute(async () =>
        {
            await _connection.EnsureConnected(cancellationToken);
            _channel ??= await _connection.CreateChannelAsync(cancellationToken);

            await _channel.QueueDeclareAsync(
                queue: _queueName, durable: true, exclusive: false, autoDelete: false, arguments: null, cancellationToken: cancellationToken);

            var message = _eventMessageSerializer.Serialize(@event);
            var body = Encoding.UTF8.GetBytes(message);

            using Activity? sendActivity = EventBusActivitySource.PublisherHasListeners
                ? EventBusActivitySource.Publish(@event.GetEventName())
                : default;
            //sendActivity?.SetTag("eventbus.message.name", GetEventName(@event));

            var properties = new BasicProperties
            {
                Persistent = true
                , AppId = _connection.ApplicationId
                , CorrelationId = null
                , Headers = null
                , MessageId = Guid.NewGuid().ToString()
            };

            await _channel.BasicPublishAsync(
                exchange: string.Empty, routingKey: _queueName, mandatory: true, basicProperties: properties, body: body);
        });
    }

    public void Dispose()
    {
        _channel?.Dispose();
        _connection.Dispose();
    }
}
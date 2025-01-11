using Arcturus.EventBus.Abstracts;
using Arcturus.EventBus.Diagnostics;
using Arcturus.EventBus.RabbitMQ.Internals;
using Microsoft.Extensions.Logging;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System.Net.Sockets;
using System.Text;

namespace Arcturus.EventBus.RabbitMQ;

public sealed class RabbitMQPublisher : IPublisher
{
    private readonly RabbitMQConnection _connection;
    private readonly string _queueName;
    private readonly ILogger<RabbitMQPublisher> _logger;

    internal RabbitMQPublisher(
        Abstracts.IConnection connection
        , ILoggerFactory loggerFactory
        , string? queueName = null)
    {
        if (connection is not RabbitMQConnection)
            throw new NotImplementedException($"Requires RabbitMQConnection");

        _connection = (RabbitMQConnection)connection;
        _queueName = queueName ?? "default_queue";

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
            .WaitAndRetry(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (exception, timeSpan) => {
                //_logger.LogWarning(exception, "Could not publish event #{EventId} after {Timeout} seconds: {ExceptionMessage}.", @event, $"{timeSpan.TotalSeconds:n1}", exception.Message);
            });

        await policy.Execute(async () => {
            await _connection.EnsureConnected(cancellationToken);

            await _connection.Channel.QueueDeclareAsync(
                queue: _queueName, durable: true, exclusive: false, autoDelete: false, arguments: null, cancellationToken: cancellationToken);

            var message = EventMessageSerializer.Serialize(@event);
            var body = Encoding.UTF8.GetBytes(message);

            using Activity? sendActivity = EventBusActivitySource.PublisherHasListeners
                ? EventBusActivitySource.Publish(GetEventName(@event))
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

            await _connection.Channel.BasicPublishAsync(
                exchange: string.Empty, routingKey: _queueName, mandatory: true, basicProperties: properties, body: body);
        });
    }

    private static string GetEventName(IEventMessage @event)
    {
        var messageAttribute = @event.GetType().GetCustomAttribute<EventMessageAttribute>();
        if (messageAttribute is not null)
            return messageAttribute.Name;

        return @event.GetType().Name;
    }
}
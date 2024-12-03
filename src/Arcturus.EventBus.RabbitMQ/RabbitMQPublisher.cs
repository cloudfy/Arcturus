using Arcturus.EventBus.Abstracts;
using Arcturus.EventBus.Diagnostics;
using Arcturus.EventBus.RabbitMQ.Internals;
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

    internal RabbitMQPublisher(Abstracts.IConnection connection, string? queueName = null)
    {
        if (connection is not RabbitMQConnection)
            throw new NotImplementedException($"Requires RabbitMQConnection");

        _connection = (RabbitMQConnection)connection;
        _queueName = queueName ?? "default_queue";
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
                // _logger.LogWarning(exception, "Could not publish event #{EventId} after {Timeout} seconds: {ExceptionMessage}.", @event.Id, $"{timeSpan.TotalSeconds:n1}", exception.Message);
            });

        await policy.Execute(async () => {
            await _connection.EnsureConnected(cancellationToken);

            await _connection.Channel.QueueDeclareAsync(
                queue: _queueName, durable: true, exclusive: false, autoDelete: false, arguments: null, cancellationToken: cancellationToken);

            var message = EventMessageSerializer.Serialize(@event);
            var body = Encoding.UTF8.GetBytes(message);

            using Activity? sendActivity = EventBusActivitySource.PublisherHasListeners
                ? EventBusActivitySource.Publish(@event.GetType().Name)
                : default;
            //sendActivity?.SetTag("eventbus.message.name", @event.GetType().Name);

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
}
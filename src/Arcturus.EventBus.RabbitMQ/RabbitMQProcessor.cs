using Arcturus.EventBus.Abstracts;
using Arcturus.EventBus.Serialization;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Arcturus.EventBus.RabbitMQ;

public sealed class RabbitMQProcessor : IProcessor, IDisposable
{
    public event Func<IEventMessage, OnProcessEventArgs?, Task>? OnProcessAsync;
    private readonly RabbitMQConnection _connection;
    private readonly string _queueName;
    private readonly int _maxDegreeOfParallelism;
    private IChannel? _channel;
    private readonly ILogger<RabbitMQProcessor> _logger;
    private readonly IEventMessageSerializer _eventMessageSerializer;

    internal RabbitMQProcessor(
        Abstracts.IConnection connection
        , ILoggerFactory loggerFactory
        , IEventMessageSerializer eventMessageSerializer
        , string? queueName = null
        , int maxDegreeOfParallelism = 1)
    {
        if (connection is not RabbitMQConnection)
            throw new NotImplementedException();

        _connection = (RabbitMQConnection)connection;
        _queueName = queueName ?? "default_queue";
        _maxDegreeOfParallelism = Math.Max(1, maxDegreeOfParallelism);
        _logger = loggerFactory.CreateLogger<RabbitMQProcessor>();
        _eventMessageSerializer = eventMessageSerializer;
    }

    public async Task WaitForEvents(CancellationToken cancellationToken = default)
    {
        // Create dedicated channel for this consumer
        _channel = await _connection.CreateChannelAsync(cancellationToken);

        _logger.LogInformation("[RabbitMQ] Starting consumer for queue: {QueueName}", _queueName);

        await _channel.QueueDeclareAsync(
            queue: _queueName
            , durable: true
            , exclusive: false
            , autoDelete: false
            , arguments: null
            , cancellationToken: cancellationToken);

        await _channel.BasicQosAsync(prefetchSize: 0, prefetchCount: (ushort)_maxDegreeOfParallelism, global: false, cancellationToken);

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            _logger.LogTrace($"[RabbitMQ] Message received!");
            try
            {
                byte[] body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                var @event = _eventMessageSerializer.Deserialize(message);

                if (OnProcessAsync is not null)
                    await OnProcessAsync.Invoke(
                        @event
                        , new OnProcessEventArgs(
                            ea.BasicProperties.MessageId
                            , ea.BasicProperties.Headers
                            , ea.CancellationToken));

                await _channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false, ea.CancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message");
                await _channel.BasicNackAsync(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true, cancellationToken: ea.CancellationToken);
            }
        };

        var consumerTag = await _channel.BasicConsumeAsync(_queueName, autoAck: false, consumer: consumer, cancellationToken);
        _logger.LogInformation($"[RabbitMQ] Consumer started: {consumerTag}");
        
        while (!cancellationToken.IsCancellationRequested)
        {
            await Task.Delay(100, cancellationToken);
        }
    }

    public void Dispose()
    {
        _channel?.Dispose();
        _connection.Dispose();
    }
}

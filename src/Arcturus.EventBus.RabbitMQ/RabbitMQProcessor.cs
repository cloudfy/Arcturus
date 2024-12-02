using Arcturus.EventBus.Abstracts;
using Arcturus.EventBus.RabbitMQ.Internals;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;

namespace Arcturus.EventBus.RabbitMQ;

public sealed class RabbitMQProcessor : IProcessor
{
    public event Func<IEventMessage, OnProcessEventArgs?, Task>? OnProcessAsync;
    private readonly RabbitMQConnection _connection;
    private readonly string _queueName;

    internal RabbitMQProcessor(Abstracts.IConnection connection, string? queueName = null)
    {
        if (connection is not RabbitMQConnection)
            throw new NotImplementedException();

        _connection = (RabbitMQConnection)connection;
        _queueName = queueName ?? "default_queue";
    }

    public async Task WaitForEvents(CancellationToken cancellationToken = default)
    {
        await _connection.EnsureConnected(cancellationToken);

        // consumer
        await _connection.Channel.QueueDeclareAsync(
            queue: _queueName
            , durable: true
            , exclusive: false
            , autoDelete: false
            , arguments: null
        , cancellationToken: cancellationToken);

        await _connection.Channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false, cancellationToken);
        // Console.WriteLine(" [*] Waiting for messages.");

        var consumer = new AsyncEventingBasicConsumer(_connection.Channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            byte[] body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            var @event = EventMessageSerializer.Deserialize(message);

            if (OnProcessAsync is not null)
                await OnProcessAsync.Invoke(
                    @event
                    , new OnProcessEventArgs(
                        ea.BasicProperties.MessageId
                        , ea.BasicProperties.Headers
                        , ea.CancellationToken));

            await _connection.Channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false, cancellationToken);
        };

        await _connection.Channel.BasicConsumeAsync(_queueName, autoAck: false, consumer: consumer, cancellationToken);
        //  await _connection.Channel.BasicConsumeAsync(queueName, false, null, false, false, null, consumer, cancellationToken);
        while (!cancellationToken.IsCancellationRequested)
        {
            cancellationToken.WaitHandle.WaitOne(100);
        }
    }
}

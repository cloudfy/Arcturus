using Arcturus.EventBus.Abstracts;
using Arcturus.EventBus.AzureStorageQueue.Internals;

namespace Arcturus.EventBus.AzureStorageQueue;

public sealed class StorageQueueProcessor : IProcessor
{
    private readonly StorageQueueConnection _connection;
    private readonly string _queueName;

    public event Func<IEventMessage, OnProcessEventArgs?, Task>? OnProcessAsync;

    internal StorageQueueProcessor(IConnection connection, string? queueName = null)
    {
        _connection = (StorageQueueConnection)connection;
        _queueName = queueName ?? "default_queue";
    }

    public async Task WaitForEvents(CancellationToken cancellationToken = default)
    {
        await _connection.EnsureConnected();

        // loop and await messages
        var queueClient = await _connection.GetQueueClient(_queueName);
        await queueClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);

        while (!cancellationToken.IsCancellationRequested)
        {
            var receiveMessages = await queueClient.ReceiveMessagesAsync(1, null, cancellationToken);

            if (receiveMessages is not null && receiveMessages.Value.Length > 0)
            {
                foreach (var receiveMessage in receiveMessages.Value)
                {
                    var messageBody = receiveMessage.Body.ToString();
                    var @event = EventMessageSerializer.Deserialize(messageBody);

                    if (OnProcessAsync is not null)
                    {
                        await OnProcessAsync.Invoke(
                            @event
                            , new OnProcessEventArgs(
                                receiveMessage.MessageId
                                , null
                                , cancellationToken));
                        await queueClient.DeleteMessageAsync(
                            receiveMessage.MessageId, receiveMessage.PopReceipt, cancellationToken);
                    }
                }
            }

            cancellationToken.WaitHandle.WaitOne(100);
        }
    }
}
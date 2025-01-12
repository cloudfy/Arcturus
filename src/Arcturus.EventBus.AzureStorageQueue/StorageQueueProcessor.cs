using Arcturus.EventBus.Abstracts;
using Arcturus.EventBus.AzureStorageQueue.Internals;

namespace Arcturus.EventBus.AzureStorageQueue;

public sealed class StorageQueueProcessor : IProcessor
{
    private readonly StorageQueueConnection _connection;
    private readonly string _queueName;
    private readonly StorageQueueOptions _options;

    public event Func<IEventMessage, OnProcessEventArgs?, Task>? OnProcessAsync;

    internal StorageQueueProcessor(IConnection connection, StorageQueueOptions storageQueueOptions, string? queueName = null)
    {
        _connection = (StorageQueueConnection)connection;
        _queueName = queueName ?? "default_queue";
        _options = storageQueueOptions;
    }

    public async Task WaitForEvents(CancellationToken cancellationToken = default)
    {
        await _connection.EnsureConnected();

        // loop and await messages
        var queueClient = await _connection.GetQueueClient(_queueName);
        await queueClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);

        while (!cancellationToken.IsCancellationRequested)
        {
            var receiveMessages = await queueClient.ReceiveMessagesAsync(
                1, _options.MessageProcessing.VisibilityTimeout, cancellationToken);

            if (receiveMessages is not null && receiveMessages.Value.Length > 0)
            {
                foreach (var receiveMessage in receiveMessages.Value)
                {
                    var messageBody = receiveMessage.Body.ToString();
                    var @event = EventMessageSerializer.Deserialize(messageBody);

                    if (OnProcessAsync is not null)
                    {
                        if (_options.MessageProcessing.DeleteMessageBeforeProcessing.GetValueOrDefault(true)) 
                        { 
                            // delete message before it resurface
                            await queueClient.DeleteMessageAsync(
                                receiveMessage.MessageId, receiveMessage.PopReceipt, cancellationToken);
                        }

                        // raise the request
                        // TODO: try/catch on exception move to dead letter
                        await OnProcessAsync.Invoke(
                            @event
                            , new OnProcessEventArgs(
                                receiveMessage.MessageId
                                , null
                                , cancellationToken));
                    }
                }
            }

            cancellationToken.WaitHandle.WaitOne(
                _options.MessageProcessing.MessageIntervalMilliseconds ?? 100);
        }
    }
}
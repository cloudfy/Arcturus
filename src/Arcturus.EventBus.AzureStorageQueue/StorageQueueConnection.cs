using Arcturus.EventBus.Abstracts;
using Azure.Storage.Queues;

namespace Arcturus.EventBus.AzureStorageQueue;

public sealed class StorageQueueConnection : IConnection
{
    private readonly string? _applicationId;
    private StorageQueueOptions _currentOptions;
    private bool _isConnected = false;
    private QueueServiceClient? _queueServiceClient;

    internal StorageQueueConnection(StorageQueueOptions currentOptions)
    {
        _applicationId = currentOptions.ApplicationId;
        _currentOptions = currentOptions;
    }

    public string? ApplicationId => _applicationId;
    public bool IsConnected => _isConnected;

    internal ValueTask Connect()
    {
        _queueServiceClient = new(
            _currentOptions.ConnectionString
            , new QueueClientOptions {
                MessageEncoding = QueueMessageEncoding.None
                , Retry = {
                    Mode = Azure.Core.RetryMode.Exponential
                    , MaxRetries = 10
                    , Delay = TimeSpan.FromSeconds(2)
                    , MaxDelay = TimeSpan.FromSeconds(30)
                }
            });

        _isConnected = true;

        return ValueTask.CompletedTask;
    }

    internal ValueTask EnsureConnected()
    {
        if (!IsConnected)
            return Connect();
        return ValueTask.CompletedTask;
    }

    internal async ValueTask<QueueClient> GetQueueClient(string queue)
    {
        await EnsureConnected();

        return _queueServiceClient!.GetQueueClient(queue);
    }
}

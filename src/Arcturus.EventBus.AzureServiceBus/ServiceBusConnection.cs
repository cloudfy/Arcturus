using Arcturus.EventBus.Abstracts;
using Azure.Messaging.ServiceBus;

namespace Arcturus.EventBus.AzureServiceBus;

public sealed class ServiceBusConnection : IConnection, IAsyncDisposable
{
    private readonly string? _applicationId;
    private readonly ServiceBusOptions _currentOptions;
    private ServiceBusClient? _client;
    private bool _isConnected = false;

    internal ServiceBusConnection(ServiceBusOptions currentOptions)
    {
        _applicationId = currentOptions.ApplicationId;
        _currentOptions = currentOptions;
    }

    public string? ApplicationId => _applicationId;
    public bool IsConnected => _isConnected;

    internal ValueTask EnsureConnected()
    {
        if (!IsConnected)
            return Connect();
        return ValueTask.CompletedTask;
    }

    internal ValueTask Connect()
    {
        ArgumentException.ThrowIfNullOrEmpty(_currentOptions.ConnectionString, nameof(_currentOptions.ConnectionString));

        var clientOptions = new ServiceBusClientOptions()
        {
            TransportType = ServiceBusTransportType.AmqpWebSockets
            ,
            Identifier = _currentOptions.ClientId
            ,
            RetryOptions = new ServiceBusRetryOptions()
            {
                Mode = ServiceBusRetryMode.Exponential
                ,
                Delay = TimeSpan.FromSeconds(1)
                ,
                MaxDelay = TimeSpan.FromSeconds(10)
                ,
                MaxRetries = 3
            }
        };
        _client = new ServiceBusClient(_currentOptions.ConnectionString, clientOptions);
        _isConnected = true;

        return ValueTask.CompletedTask;
    }

    internal async Task<ServiceBusClient> GetServiceBusClient()
    {
        await EnsureConnected();
        return _client!;
    }
    public ValueTask DisposeAsync() => _client!.DisposeAsync();
}
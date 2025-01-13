using Arcturus.EventBus.Abstracts;
using Azure.Messaging.ServiceBus;
using System;

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
        var clientOptions = new ServiceBusClientOptions()
        {
            TransportType = ServiceBusTransportType.AmqpWebSockets
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
    public ValueTask DisposeAsync()
    {
        return _client!.DisposeAsync();
    }
}
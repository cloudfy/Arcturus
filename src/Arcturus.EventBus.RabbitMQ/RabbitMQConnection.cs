using Arcturus.EventBus.Abstracts;
using Arcturus.EventBus.Threading;
using RMQ = RabbitMQ.Client;

namespace Arcturus.EventBus.RabbitMQ;

public sealed class RabbitMQConnection : IConnection
{
    private readonly string? _applicationId;
    private readonly string _clientName;
    private readonly string _connectionHostName;
    private RMQ.IConnection? _connection;
    private bool _isConnected = false;
    public string? ApplicationId => _applicationId;
    public bool IsConnected => _isConnected;

    private readonly AsyncLock _asyncLock = new();

    internal RabbitMQConnection(RabbitMQEventBusOptions options)
    {
        _applicationId = options.ApplicationId;
        _clientName = options.ClientName ?? Environment.MachineName;
        _connectionHostName = options.ConnectionString ?? "localhost";
    }

    internal async Task<RMQ.IChannel> CreateChannelAsync(CancellationToken cancellationToken = default)
    {
        await EnsureConnected(cancellationToken);
        return await _connection!.CreateChannelAsync(cancellationToken: cancellationToken);
    }

    internal async Task Connect(CancellationToken cancellationToken = default)
    {
        var factory = new RMQ.ConnectionFactory();

        // Check if HostName is a URI
        if (Uri.TryCreate(_connectionHostName, UriKind.Absolute, out var uri))
        {
            factory.Uri = uri;
        }
        else
        {
            factory.HostName = _connectionHostName;
        }

        factory.ClientProvidedName = _clientName;
        using (await _asyncLock.LockAsync(cancellationToken))
        {
            if (_connection is null || !_isConnected)
            {
                _connection?.Dispose();
                _connection = await factory.CreateConnectionAsync(_clientName, cancellationToken);
                _connection.ConnectionShutdownAsync += (sender, args) =>
                {
                    _isConnected = false;
                    return Task.CompletedTask;
                };
                _isConnected = true;
            }
        }
    }
    internal Task EnsureConnected(CancellationToken cancellationToken = default)
    {
        if (!IsConnected)
            return Connect(cancellationToken);
        return Task.CompletedTask;
    }
    public void Dispose()
    {
        _connection?.Dispose();
    }
}

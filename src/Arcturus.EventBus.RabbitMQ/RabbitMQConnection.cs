using Arcturus.EventBus.Abstracts;
using Arcturus.EventBus.Threading;
using RMQ = RabbitMQ.Client;

namespace Arcturus.EventBus.RabbitMQ;

public sealed class RabbitMQConnection : IConnection
{
    private string? _applicationId { get; }
    private string _clientName { get; }
    private string _connectionHostName { get; }
    private RMQ.IChannel? _channel;
    private RMQ.IConnection? _connection;
    private bool _isConnected = false;
    public string? ApplicationId => _applicationId;
    public bool IsConnected => _isConnected;

    private readonly AsyncLock _asyncLock = new();

    internal RMQ.IChannel Channel => _channel!;

    internal RabbitMQConnection(RabbitMQEventBusOptions options)
    {
        _applicationId = options.ApplicationId;
        _clientName = options.ClientName ?? Environment.MachineName;
        _connectionHostName = options.ConnectionString ?? "localhost";
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
            _connection = await factory.CreateConnectionAsync(_clientName, cancellationToken);
            _connection.ConnectionShutdownAsync += (sender, args) =>
            {
                _isConnected = false;
                return Task.CompletedTask;
            };
            _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);

            _isConnected = true;
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
        _channel?.Dispose();
        _connection?.Dispose();
    }
}

using Arcturus.EventBus.Abstracts;

namespace Arcturus.EventBus.Sqlite;

public sealed class SqliteProcessor : IProcessor
{
    private readonly SqliteConnection _connection;
    private readonly string _queueName;
    private readonly int _pullInterval;

    internal SqliteProcessor(IConnection connection, string? queueName, int pullInterval)
    {
        if (connection is not SqliteConnection)
            throw new NotImplementedException($"Requires SqliteConnection");

        _connection = (SqliteConnection)connection;
        _queueName = queueName ?? "default_queue";
        _pullInterval = pullInterval;
    }

    public event Func<IEventMessage, OnProcessEventArgs?, Task>? OnProcessAsync;

    public Task WaitForEvents(CancellationToken cancellationToken = default)
    {
        return Task.Run(async () => {
            while (!cancellationToken.IsCancellationRequested)
            {
                var events = await _connection.PullEvents(_queueName, 5, cancellationToken);
                foreach (var @event in events)
                {
                    var args = new OnProcessEventArgs(@event.MessageId, null, cancellationToken);
                    if (OnProcessAsync != null)
                    {
                        await OnProcessAsync.Invoke(@event.Message, args);
                    }
                }
                cancellationToken.WaitHandle.WaitOne(_pullInterval);
            }
        }, cancellationToken);
    }
}

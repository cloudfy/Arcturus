namespace Arcturus.EventBus.Abstracts;

public sealed class OnProcessEventArgs : EventArgs
{
    public OnProcessEventArgs(
        string? messageId
        , IDictionary<string, object?>? headers
        , CancellationToken cancellationToken = default)
    {
        MessageId = messageId;
        Headers = headers;
        CancellationToken = cancellationToken;
    }
    /// <summary>
    /// Gets a dictionary of headers or null.
    /// </summary>
    public IDictionary<string, object?>? Headers { get; }
    /// <summary>
    /// Gets an id of the message or null.
    /// </summary>
    public string? MessageId { get; }
    /// <summary>
    /// Gets a cancellation token or null.
    /// </summary>
    public CancellationToken? CancellationToken { get; }
}
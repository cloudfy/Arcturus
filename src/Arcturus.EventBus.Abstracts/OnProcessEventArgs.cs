namespace Arcturus.EventBus.Abstracts;

/// <summary>
/// Represents the event arguments for the <see cref="IProcessor.OnProcessAsync" /> event.
/// </summary>
public sealed class OnProcessEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OnProcessEventArgs" /> class.
    /// </summary>
    /// <param name="messageId">Optional. Id of message.</param>
    /// <param name="headers">Optional. Dictionary of headers.</param>
    /// <param name="cancellationToken">Propogates notification that events should be cancelled.</param>
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
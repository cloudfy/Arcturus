namespace Arcturus.EventBus.Diagnostics;

/// <summary>
/// Represents diagnostic properties for an event bus message.
/// </summary>
public interface IDiagnosticProperties
{
    /// <summary>
    /// Gets the application identifier.
    /// </summary>
    string? AppId { get; }

    /// <summary>
    /// Gets the message identifier.
    /// </summary>
    string? MessageId { get; }

    /// <summary>
    /// Gets the correlation identifier.
    /// </summary>
    string? CorrelationId { get; }

    /// <summary>
    /// Gets the headers associated with the message.
    /// </summary>
    IDictionary<string, object?>? Headers { get; }
}

namespace Arcturus.EventBus.Diagnostics;

public interface IDiagnosticProperties
{
    string? AppId { get; }
    string? MessageId { get; }
    string? CorrelationId { get; }
    IDictionary<string, object?>? Headers { get; }
}
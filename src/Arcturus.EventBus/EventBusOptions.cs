namespace Arcturus.EventBus;

public class EventBusOptions
{
    /// <summary>
    /// Gets or sets an application id.
    /// </summary>
    public string? ApplicationId { get; set; }
    /// <summary>
    /// Gets or sets a client name.
    /// </summary>
    public string? ClientName { get; set; }
    /// <summary>
    /// Gets or sets a default exchange name. Defaults to "events".
    /// </summary>
    public string? DefaultQueueName { get; set; }
    /// <summary>
    /// If true, events will be processed by the <see cref="Arcturus.EventBus.EventHandlersProcessor"/> or fallback to the <see cref="Arcturus.EventBus.Abstracts.IProcessor.OnProcessAsync"/> event. Default true.
    /// </summary>
    /// <remarks>
    /// The <see cref="Arcturus.EventBus.EventHandlersProcessor"/> provide support for event middleware pipelines. Use <see cref="Arcturus.EventBus.Middleware.HostExtensions.UseEventMiddleware{TMiddleware}(Microsoft.Extensions.Hosting.IHost, object?[])"/>.
    /// </remarks>
    public bool? UseEventHandlersProcessor { get; set; }
    /// <summary>
    /// Gets or sets the maximum number of events processed concurrently. A value of 1 (default) preserves
    /// sequential processing. Values greater than 1 enable parallel dispatch; the broker-level prefetch
    /// is automatically aligned to this value.
    /// </summary>
    public int MaxDegreeOfParallelism { get; set; } = 1;
}
namespace Arcturus.EventBus.AzureServiceBus;

public class ServiceBusOptions
{
    /// <summary>
    /// Gets or sets the connection string to the ServiceBus.
    /// </summary>
    public string ConnectionString { get; set; } = null!;
    /// <summary>
    /// Gets or sets an application name.
    /// </summary>
    public string? ApplicationId { get; set; }
    /// <summary>
    /// Gets or sets the clientId of the ServiceBus client. This is used for logging.
    /// </summary>
    public string? ClientId { get; set; }
    /// <summary>
    /// If true, events will be processed by the <see cref="Arcturus.EventBus.EventHandlersProcessor"/> or fallback to the <see cref="Arcturus.EventBus.Abstracts.IProcessor.OnProcessAsync"/> event.
    /// </summary>
    /// <remarks>
    /// The <see cref="Arcturus.EventBus.EventHandlersProcessor"/> provide support for event middleware pipelines. Use <see cref="Arcturus.EventBus.Middleware.HostExtensions.UseEventMiddleware{TMiddleware}(Microsoft.Extensions.Hosting.IHost, object?[])"/>.
    /// </remarks>
    public bool? UseEventHandlersProcessor { get; set; }
    /// <summary>
    /// Gets or sets the default queue. If no queue is specified the default queue is 'default_queue'.
    /// </summary>
    public string? DefaultQueueName { get; set; }
    /// <summary>
    /// Gets or sets the interval in milliseconds to pull messages from the queue.
    /// </summary>
    public int? PullIntervalMilliseconds { get; set; }
}

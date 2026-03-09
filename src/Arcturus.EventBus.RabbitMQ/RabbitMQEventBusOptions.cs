namespace Arcturus.EventBus.RabbitMQ;

public sealed class RabbitMQEventBusOptions
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
    /// Gets or sets a host name. Defaults to localhost.
    /// </summary>
    [Obsolete("HostName is deprecated. Use ConnectionString instead.")]
    public string? HostName { get; set; }
    /// <summary>
    /// Gets or sets the amqps:// connection string. If provided, it will override the HostName property.
    /// </summary>
    public string? ConnectionString { get; set; }
    /// <summary>
    /// If true, events will be processed by the <see cref="Arcturus.EventBus.EventHandlersProcessor"/> or fallback to the <see cref="Arcturus.EventBus.Abstracts.IProcessor.OnProcessAsync"/> event. Default true.
    /// </summary>
    /// <remarks>
    /// The <see cref="Arcturus.EventBus.EventHandlersProcessor"/> provide support for event middleware pipelines. Use <see cref="Arcturus.EventBus.Middleware.HostExtensions.UseEventMiddleware{TMiddleware}(Microsoft.Extensions.Hosting.IHost, object?[])"/>.
    /// </remarks>
    public bool? UseEventHandlersProcessor { get; set; }
}
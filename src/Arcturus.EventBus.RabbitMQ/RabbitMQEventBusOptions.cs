namespace Arcturus.EventBus.RabbitMQ;

public sealed class RabbitMQEventBusOptions
{
    internal string? ApplicationId { get; set; } 
    internal string? ClientName { get; set; }
    internal bool UseEventHandlersProcessor { get; set; }
    internal int MaxDegreeOfParallelism { get; set; } = 1;

    /// <summary>
    /// Gets or sets a default exchange name. Defaults to "events".
    /// </summary>
    public string? DefaultQueueName { get; set; }
    /// <summary>
    /// Gets or sets the amqps:// connection string. If provided, it will override the HostName property.
    /// </summary>
    public string? ConnectionString { get; set; }
}
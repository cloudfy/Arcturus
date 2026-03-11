namespace Arcturus.EventBus.AzureServiceBus;

public class ServiceBusOptions
{
    internal string? ApplicationId { get; set; }
    internal string? ClientId { get; set; }
    internal bool UseEventHandlersProcessor { get; set; }

    /// <summary>
    /// Gets or sets the connection string to the ServiceBus.
    /// </summary>
    public string ConnectionString { get; set; } = null!;
    
    /// <summary>
    /// Gets or sets the default queue. If no queue is specified the default queue is 'default_queue'.
    /// </summary>
    public string? DefaultQueueName { get; set; }
    /// <summary>
    /// Gets or sets the interval in milliseconds to pull messages from the queue.
    /// </summary>
    public int? PullIntervalMilliseconds { get; set; }
}

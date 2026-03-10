namespace Arcturus.EventBus.AzureServiceBus;

public class ServiceBusOptions : EventBusOptions
{
    /// <summary>
    /// Gets or sets the connection string to the ServiceBus.
    /// </summary>
    public string ConnectionString { get; set; } = null!;
     
    /// <summary>
    /// Gets or sets the interval in milliseconds to pull messages from the queue.
    /// </summary>
    public int? PullIntervalMilliseconds { get; set; }
}

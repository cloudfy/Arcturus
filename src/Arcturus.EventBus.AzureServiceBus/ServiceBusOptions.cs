namespace Arcturus.EventBus.AzureServiceBus;

public class ServiceBusOptions
{
    public string ConnectionString { get; set; } = null!;
    public string? ApplicationId { get; set; }
    /// <summary>
    /// Gets or sets the clientId of the ServiceBus client. This is used for logging.
    /// </summary>
    public string? ClientId { get; set; }
    public bool? UseEventHandlersProcessor { get; set; }
    public string? DefaultQueueName { get; set; }
    public int? PullIntervalMilliseconds { get; set; }
}

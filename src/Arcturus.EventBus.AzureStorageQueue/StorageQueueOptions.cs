namespace Arcturus.EventBus.AzureStorageQueue;

public class StorageQueueOptions
{
    public string? ApplicationId { get; set; }
    /// <summary>
    /// Gets or sets the Azure Storage Queue connection string.
    /// </summary>
    public string ConnectionString { get; set; } = null!;
    /// <summary>
    /// Gets or sets the default queue name.
    /// </summary>
    public string? DefaultQueueName { get; set; }
    public bool? UseEventHandlersProcessor { get; set; }
    public MessageProcessing MessageProcessing { get; set; } = new();
}
public class MessageProcessing
{
    /// <summary>
    /// Specifies the visibility timeout of the Azure Storage Queue. Default is 30 seconds.
    /// </summary>
    public TimeSpan? VisibilityTimeout { get; set; }
    /// <summary>
    /// Specifies if the event message is deleted before processing. Defaults to true.
    /// </summary>
    public bool? DeleteMessageBeforeProcessing { get; set; }
    /// <summary>
    /// Specifies the interval between every message retrieval in milliseconds. Defaults to 100.
    /// </summary>
    public int? MessageIntervalMilliseconds { get; set; }
}
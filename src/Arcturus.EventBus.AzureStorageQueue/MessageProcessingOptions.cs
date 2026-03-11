namespace Arcturus.EventBus.AzureStorageQueue;

public class MessageProcessingOptions
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
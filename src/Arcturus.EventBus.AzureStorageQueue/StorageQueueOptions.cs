namespace Arcturus.EventBus.AzureStorageQueue;

public class StorageQueueOptions
{
    internal bool UseEventHandlersProcessor { get; set; }
    internal string? ApplicationId { get; set; }

    /// <summary>
    /// Gets or sets the Azure Storage Queue connection string.
    /// </summary>
    public string ConnectionString { get; set; } = null!;
    /// <summary>
    /// Gets or sets the default queue name.
    /// </summary>
    public string? DefaultQueueName { get; set; }
    public MessageProcessingOptions MessageProcessing { get; set; } = new();
}

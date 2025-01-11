namespace Arcturus.EventBus.AzureStorageQueue;

public class StorageQueueOptions
{
    public string? ApplicationId { get; set; }
    public string ConnectionString { get; set; } = null!;
    public string? DefaultQueueName { get; set; }
    public bool? UseEventHandlersProcessor { get; set; }
}

using Azure;

namespace Arcturus.Extensions.Caching.AzureStorageTable.Models;

public class CacheItem : Azure.Data.Tables.ITableEntity
{
    public CacheItem() { }
    public CacheItem(string partitionKey, string rowKey)
    {
        PartitionKey = partitionKey;
        RowKey = rowKey;
    }

    public string PartitionKey { get; set; }
    public string RowKey { get; set; }

    public DateTimeOffset? Timestamp { get; set; }
    public DateTimeOffset ExpiresAtTime { get; set; }
    public DateTimeOffset LastAccessTime { get; set; }
    public ETag ETag { get; set; } = ETag.All;
    public byte[]? Data { get; set; }
}

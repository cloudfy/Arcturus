using System.ComponentModel.DataAnnotations;

namespace Arcturus.Extensions.Caching.AzureStorageTable;

public class AzureStorageTableCacheOptions
{
    /// <summary>
    /// The name of the table to use.
    /// </summary>
    [Required]
    public string TableName { get; set; } = null!;
    /// <summary>
    /// The Partition Key to use.
    /// </summary>
    [Required]
    public string PartitionKey { get; set; } = null!;
    /// <summary>
    /// Gets or sets the connection string of the storage account.
    /// </summary>
    public string? ConnectionString { get; set; }
    /// <summary>
    /// This is likely to be similar to "https://{account_name}.table.core.windows.net/" or "https://{account_name}.table.cosmos.azure.com/".
    /// </summary>
    public string? TableServiceEndpoint { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether the table should be created if it does not already exist.
    /// </summary>
    public bool CreateTableIfNotExists { get; set; } = true;
    /// <summary>
    /// Gets or sets a value indicating periodic interval to scan and delete expired items in the cache. Default this is set to null, which means that this functionality is disabled.
    /// </summary>
    public TimeSpan? ExpiredItemsDeletionInterval { get; set; }
    /// <summary>
    /// Gets or set the default expiration hours. Default to 24 hours.
    /// </summary>
    public int? DefaultExpirationHours { get; set; }
}

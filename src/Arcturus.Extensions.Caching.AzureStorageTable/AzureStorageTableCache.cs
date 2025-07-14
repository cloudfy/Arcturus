using Arcturus.Extensions.Caching.AzureStorageTable.Models;
using Azure;
using Azure.Data.Tables;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Internal;

namespace Arcturus.Extensions.Caching.AzureStorageTable;

/// <summary>
/// Provides a distributed caching implementation using Azure Table Storage.
/// </summary>
/// <remarks>This class implements the <see cref="IDistributedCache"/> interface, allowing applications to store
/// and retrieve cached data in Azure Table Storage. It supports features such as expiration policies, automatic removal
/// of expired items, and lazy initialization of the underlying table client.</remarks>
public sealed class AzureStorageTableCache : IDistributedCache
{
    private readonly Lazy<TableClient> _tableClient;
    private readonly AzureStorageTableCacheOptions _options;
    private readonly ISystemClock _systemClock;

    private readonly TimeSpan? _expiredItemsDeletionInterval;
    private readonly Func<Task> _deleteExpiredCachedItemsDelegate;
    private readonly string _partitionKey;
    private DateTimeOffset _lastExpirationScan;

    public AzureStorageTableCache(
        AzureStorageTableCacheOptions options
        , ISystemClock systemClock)
    {
        ArgumentNullException.ThrowIfNull(options.TableName);
        ArgumentNullException.ThrowIfNull(options);

        _options = options;
        _systemClock = systemClock ?? throw new ArgumentNullException(nameof(systemClock));
        _expiredItemsDeletionInterval = options.ExpiredItemsDeletionInterval;
        _deleteExpiredCachedItemsDelegate = DeleteExpiredCacheItems;
        _partitionKey = options.PartitionKey;

        _tableClient = new Lazy<TableClient>(() => 
        {
            TableServiceClient serviceClient;

            if (string.IsNullOrWhiteSpace(options.ConnectionString) == false)
            {
                serviceClient = new TableServiceClient(
                    options.ConnectionString
                    , new TableClientOptions { });
            }
            else if (string.IsNullOrWhiteSpace(options.TableServiceEndpoint) == false)
            {
                serviceClient = new TableServiceClient(
                    new Uri(options.TableServiceEndpoint)
                    , new TableClientOptions { });
            }
            else
            {
                throw new ArgumentException("Either ConnectionString or TableServiceEndpoint must be provided.");
            }

            var tableClient = serviceClient.GetTableClient(
                tableName: options.TableName
            );

            if (_options.CreateTableIfNotExists)
            {
                tableClient.CreateIfNotExists();
            }
            return tableClient;
        }); 
    }
    public byte[]? Get(string key)
        => GetAsync(key).ConfigureAwait(false).GetAwaiter().GetResult();

    public async Task<byte[]?> GetAsync(string key, CancellationToken token = default)
    {
        try
        {
            var item = await Retrieve(key, token);
            RemoveExpiredItemsIfRequired();

            return item?.Data;
        }
        catch (RequestFailedException ex) when (ex.Status == 404)
        {
            // Item not found, return null
            return null;
        }
        catch
        {
            throw;
        }
    }

    public void Refresh(string key)
        => RefreshAsync(key).ConfigureAwait(false).GetAwaiter().GetResult();

    public async Task RefreshAsync(string key, CancellationToken cancellationToken = default)
    {
        var item = await Retrieve(key, cancellationToken);
        if (item is not null)
        {
            if (ShouldDelete(item))
            {
                await RemoveAsync(key, cancellationToken);
                return;
            }
            
            item.LastAccessTime = DateTimeOffset.UtcNow;

            await _tableClient.Value.UpsertEntityAsync<CacheItem>(item, TableUpdateMode.Replace, cancellationToken);
        }

        RemoveExpiredItemsIfRequired();
    }

    public void Remove(string key)
        => RemoveAsync(key).ConfigureAwait(false).GetAwaiter().GetResult();
    
    public async Task RemoveAsync(string key, CancellationToken token = default)
    {
        try
        {
            await _tableClient.Value.DeleteEntityAsync(_options.PartitionKey, key, cancellationToken: token);
        }
        catch
        {
            throw;
        }
    }
    public void Set(string key, byte[] value, DistributedCacheEntryOptions options)
        => SetAsync(key, value, options).ConfigureAwait(false).GetAwaiter().GetResult();

    public async Task SetAsync(
        string key
        , byte[] value
        , DistributedCacheEntryOptions options
        , CancellationToken cancellationToken = default)
    {
        var utcNow = _systemClock.UtcNow;
        var expiresAtTime = GetExpiryTime(options, utcNow);

        var cacheItem = new CacheItem(_partitionKey, key) {
            Data = value
            , ExpiresAtTime = expiresAtTime
            , LastAccessTime = utcNow
            , Timestamp = utcNow
        };

        await _tableClient.Value.UpsertEntityAsync<CacheItem>(cacheItem, TableUpdateMode.Replace, cancellationToken);
    }

    private DateTimeOffset GetExpiryTime(
        DistributedCacheEntryOptions options
        , DateTimeOffset currentTime)
    {
        if (options.AbsoluteExpirationRelativeToNow.HasValue)
        {
            return currentTime.Add(options.AbsoluteExpirationRelativeToNow.Value);
        }
        if (options.AbsoluteExpiration.HasValue)
        {
            if (options.AbsoluteExpiration.Value <= currentTime)
            {
                throw new ArgumentOutOfRangeException(nameof(options.AbsoluteExpiration), options.AbsoluteExpiration.Value, "The absolute expiration value must be in the future.");
            }

            return options.AbsoluteExpiration.Value;
        }
        // default one day
        return currentTime.AddHours(_options.DefaultExpirationHours ?? 24);
    }

    private void RemoveExpiredItemsIfRequired()
    {
        var utcNow = _systemClock.UtcNow;

        if (utcNow - _lastExpirationScan > _expiredItemsDeletionInterval)
        {
            _lastExpirationScan = utcNow;
            Task.Run(_deleteExpiredCachedItemsDelegate);
        }
    }

    private async Task<CacheItem?> Retrieve(string key, CancellationToken token)
    {
        try
        {
            Response<CacheItem> response = await _tableClient.Value.GetEntityAsync<CacheItem>(
                partitionKey: _partitionKey
                , rowKey: key
                , cancellationToken: token
            );

            if (response.Value.ExpiresAtTime < _systemClock.UtcNow)
                return null;

            return response.Value;
        }
        catch (RequestFailedException ex) when (ex.Status == 404)
        {
            // Item not found, return null
            return null;
        }
        catch
        {
            throw;
        }
    }
    private async Task DeleteExpiredCacheItems()
    {
        var utcNow = _systemClock.UtcNow;

        try
        {
            var itemsToDelete = _tableClient.Value.QueryAsync<CacheItem>(
            _ => _.PartitionKey == _options.PartitionKey && _.ExpiresAtTime <= utcNow);

            await foreach (var item in itemsToDelete)
            {
                await _tableClient.Value.DeleteEntityAsync(item);
            }
        }
        catch
        {
            // Just ignore any exceptions
        }
    }

    private bool ShouldDelete(CacheItem item)
    {
        var utcNow = _systemClock.UtcNow;

        return item.ExpiresAtTime <= utcNow;
    }
}

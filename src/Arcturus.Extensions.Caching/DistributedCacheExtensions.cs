using Arcturus.Extensions.Caching.Internals;
using Microsoft.Extensions.Caching.Distributed;

namespace Arcturus.Extensions.Caching;

public static class DistributedCacheExtensions
{
    /// <summary>
    /// Retrieves an item from the distributed cache or creates and stores it if it does not exist.
    /// </summary>
    /// <remarks>This method first attempts to retrieve the item from the cache using the specified <paramref
    /// name="key"/>.  If the item is not found, the <paramref name="factory"/> function is invoked to create the item, 
    /// which is then stored in the cache with the specified <paramref name="options"/>.</remarks>
    /// <typeparam name="T">The type of the item to retrieve or create. Must be a reference type.</typeparam>
    /// <param name="distributedCache">The distributed cache instance to retrieve or store the item.</param>
    /// <param name="key">The unique key identifying the cached item. Cannot be null or empty.</param>
    /// <param name="factory">A factory function used to create the item if it is not found in the cache.  The function is invoked only if the
    /// item does not already exist.</param>
    /// <param name="options">Optional cache entry options that specify expiration and other settings for the cached item.  If null, default
    /// cache options are used.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the operation to complete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the cached or newly created item.</returns>
    public static async Task<T> GetOrSetItem<T>(
        this IDistributedCache distributedCache
        , string key
        , Func<T> factory
        , DistributedCacheEntryOptions? options = null
        , CancellationToken cancellationToken = default)
        where T : class
    {
        var item = await GetItem<T>(distributedCache, key, cancellationToken);
        if (item is not null)
            return item;

        var factoryItem = factory();
        await SetItem<T>(distributedCache, key, factoryItem, options, cancellationToken);
        return factoryItem;
    }
    public static async Task<T?> GetItem<T>(
        this IDistributedCache distributedCache
        , string key
        , CancellationToken cancellationToken = default)
        where T : class
    {
        if (typeof(T) == typeof(string))
        {
            var stringValue = await distributedCache.GetStringAsync(key, cancellationToken).ConfigureAwait(false);
            if (stringValue == null)
            {
                return null;
            }
            return (T)Convert.ChangeType(stringValue, typeof(T));
        }

        var result = await distributedCache.GetAsync(key, cancellationToken).ConfigureAwait(false);
        if (result is null)
            return null;

        return BinarySerializer.Deserialize<T>(result);
    }
    /// <summary>
    /// Asynchronously sets a value in the distributed cache with the specified key.
    /// </summary>
    /// <remarks>If <typeparamref name="T"/> is not <see cref="string"/>, the value is serialized using a
    /// binary serializer before being stored. Ensure that the type <typeparamref name="T"/> is compatible with the
    /// serialization mechanism used.</remarks>
    /// <typeparam name="T">The type of the value to store in the cache. Must be a reference type.</typeparam>
    /// <param name="distributedCache">The <see cref="IDistributedCache"/> instance used to store the value.</param>
    /// <param name="key">The key under which the value will be stored. Cannot be <see langword="null"/> or empty.</param>
    /// <param name="value">The value to store in the cache. If <typeparamref name="T"/> is <see cref="string"/>, the value is stored as a
    /// string; otherwise, it is serialized before being stored.</param>
    /// <param name="options">Optional cache entry options that specify expiration and other settings. If <see langword="null"/>, default
    /// options are used.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the operation to complete.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
    public static Task SetItem<T>(
        this IDistributedCache distributedCache
        , string key
        , T value
        , DistributedCacheEntryOptions? options = null
        , CancellationToken cancellationToken = default)
        where T : class
    {
        if (options is null)
        {
            return typeof(T) == typeof(string) ?
                distributedCache.SetStringAsync(key, value as string, cancellationToken) :
                distributedCache.SetAsync(key, BinarySerializer.Serialize(value), cancellationToken);
        }
        else
        {
            return typeof(T) == typeof(string) ?
                distributedCache.SetStringAsync(key, value as string, options, cancellationToken) :
                distributedCache.SetAsync(key, BinarySerializer.Serialize(value), options, cancellationToken);
        }
    }
}

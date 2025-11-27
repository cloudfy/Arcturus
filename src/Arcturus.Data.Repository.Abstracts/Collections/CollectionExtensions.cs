namespace Arcturus.Collections;


#if NET10_0_OR_GREATER
// not used
#else
/// <summary>
/// Provides common extensions for <see cref="IAsyncEnumerable{T}"/>.
/// </summary>
public static class CollectionExtensions
{
    /// <summary>
    /// Converts the <paramref name="source"/> to a <see cref="List{T}"/> using asynchronious execution.
    /// </summary>
    /// <typeparam name="TSource">Type fo resource.</typeparam>
    /// <param name="source">Required. Source to convert.</param>
    /// <param name="cancellationToken">Optional. Propogates notification that operations should be cancelled.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">Source is null.</exception>
    public async static ValueTask<List<TSource>> ToListAsync<TSource>(
        this IAsyncEnumerable<TSource> source
        , CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(source, nameof(source));
        var list = new List<TSource>();

        await foreach (var item in source
            .WithCancellation(cancellationToken)
            .ConfigureAwait(false))
        {
            list.Add(item);
        }

        return list;
    }
    /// <summary>
    /// Converts the <paramref name="source"/> to an <see cref="Array"/> using asynchronious execution.
    /// </summary>
    /// <typeparam name="TSource">Type fo resource.</typeparam>
    /// <param name="source">Required. Source to convert.</param>
    /// <param name="cancellationToken">Optional. Propogates notification that operations should be cancelled.</param>
    /// <returns><typeparamref name="TSource"/></returns>
    /// <exception cref="ArgumentNullException">Source is null.</exception>
    public async static ValueTask<TSource[]> ToArrayAsync<TSource>(
        this IAsyncEnumerable<TSource> source
        , CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(source, nameof(source));
        var list = await ToListAsync(source, cancellationToken);
        return [.. list];
    }
    /// <summary>
    /// Converts the <paramref name="source"/> to a <see cref="Dictionary{TKey, TValue}"/> using asynchronious execution.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TK"></typeparam>
    /// <typeparam name="TV"></typeparam>
    /// <param name="source">Required. Source to convert.</param>
    /// <param name="keySelector">Required. Predicate to select key.</param>
    /// <param name="valueSelector">Required. Predicate to select value.</param>
    /// <param name="cancellationToken">Optional. Propogates notification that operations should be cancelled.</param>
    /// <returns><see cref="Dictionary{TKey, TValue}"/></returns>
    public async static ValueTask<Dictionary<TK, TV>> ToDictionaryAsync<TSource, TK, TV>(
        this IAsyncEnumerable<TSource> source
        , Func<TSource, TK> keySelector
        , Func<TSource, TV> valueSelector
        , CancellationToken cancellationToken = default) where TK : notnull
    {
        ArgumentNullException.ThrowIfNull(source, nameof(source));

        Dictionary<TK, TV> dictionary = [];
        await foreach (var item in source
            .WithCancellation(cancellationToken)
            .ConfigureAwait(false))
        {
            dictionary.Add(keySelector(item), valueSelector(item));
        }
        return dictionary;
    }
}
#endif

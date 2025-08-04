using Arcturus.Repository.Pagination.Internals;
using Arcturus.Repository.Specification;
using Arcturus.Repository.Specification.Parsing;

namespace Arcturus.Repository.Pagination;

/// <summary>
/// Provides extension methods for repositories to support advanced querying and pagination.
/// </summary>
/// <remarks>This static class contains methods that extend the functionality of repositories, enabling features
/// such as keyset pagination and flexible filtering. These methods are designed to work with repositories implementing
/// the <see cref="IRepository{T, TKey}"/> interface.</remarks>
public static class RepositoryExtensions
{
    /// <summary>
    /// Retrieves a paginated list of results using keyset pagination, based on the specified criteria and projection.
    /// </summary>
    /// <remarks>This method uses keyset pagination to efficiently retrieve a subset of results based on the
    /// specified criteria. If an <paramref name="after"/> cursor is provided, the results will start from the entity
    /// corresponding to that cursor. The <paramref name="orderBy"/> parameter determines the primary ordering of the
    /// results, and the entity's ID is used as a secondary ordering criterion to ensure stable pagination.</remarks>
    /// <typeparam name="T">The type of the entity in the repository.</typeparam>
    /// <typeparam name="TKey">The type of the primary key of the entity.</typeparam>
    /// <typeparam name="TResult">The type of the projected result.</typeparam>
    /// <param name="repository">The repository to query for the entities.</param>
    /// <param name="predicate">An optional predicate to filter the entities. If null, no filtering is applied.</param>
    /// <param name="projection">An expression defining the projection to transform the entity into the result type.</param>
    /// <param name="keySelector">A function to extract the key from the projected result, used for pagination.</param>
    /// <param name="orderBy">The name of the property to order the results by. If null or empty, results are ordered by the entity's ID.</param>
    /// <param name="after">An optional cursor indicating the starting point for the next page of results. If null, results start from the
    /// beginning.</param>
    /// <param name="limit">The maximum number of results to return. Defaults to 20.</param>
    /// <param name="where">An optional array of additional filtering conditions in string format. If null, no additional filters are
    /// applied.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="PagingResultsList{TResult}"/> containing the paginated results, the total count of matching
    /// entities, and a cursor for retrieving the next page of results.</returns>
    public static async Task<PagingResultsList<TResult>> KeySetResult<T, TKey, TResult>(
        this IRepository<T, TKey> repository
        , Expression<Func<T, bool>>? predicate
        , Expression<Func<T, TResult>> projection
        , Func<TResult, TKey> keySelector
        , string? orderBy
        , string? after = null
        , int limit = 20
        , string[]? where = null
        , CancellationToken cancellationToken = default
        )
        where T : IEntity<TKey>
        where TKey : notnull
    {
        var querySpecification = new Specification<T, TResult>();
        if (predicate is not null)
            querySpecification.Where(predicate);

        _ = PagingCursor<TKey>.TryParse(after, out PagingCursor<TKey>? afterCursor);

        if (!string.IsNullOrEmpty(orderBy))
        {
            querySpecification.OrderBy(orderBy);
        }

        if (afterCursor is not null && !string.IsNullOrEmpty(orderBy) && afterCursor.OrderByValue is not null)
        {
            querySpecification.Where(QueryableExtensions.KeysetPagePredicate<T>(
                (orderBy, (object)afterCursor.OrderByValue),
                ("ID", (object)afterCursor.DefaultValue)
            ));

        }
        else if (afterCursor is not null)
        {
            querySpecificiation.Where($"ID ge '{afterCursor.DefaultValue}'");
        }

        querySpecificiation
            .OrderBy(_ => _.Id)
            .Take(limit + 1) // last +1
            .WhereRange(where)
            .Project(projection);

        var results = await repository
            .FindMany(querySpecificiation)
            .ToArrayAsync(cancellationToken);

        if (results.Length > 0)
        {
            var resultCount = await repository.Count(
                querySpecificiation.Clear().WhereRange(where).WhereIfNotNull(predicate)
                , cancellationToken);

            var lastItem = results.Last();
            var key1 = keySelector(lastItem);

            object? orderByValue = null;
            if (orderBy is not null)
            {
                // TODO: cache
                var prop = typeof(TResult).GetProperty(
                    orderBy
                    , System.Reflection.BindingFlags.IgnoreCase |
                        System.Reflection.BindingFlags.Public |
                        System.Reflection.BindingFlags.Instance);
                orderByValue = prop?.GetValue(lastItem);
            }

            var afterCursorValue = PagingCursor<TKey>.Create(key1, orderByValue).ToString();
            return new PagingResultsList<TResult>(results, resultCount, limit, afterCursorValue);
        }

        return PagingResultsList<TResult>.Empty();
    }

    /// <summary>
    /// Retrieves a paginated list of entities from the repository using keyset pagination.
    /// </summary>
    /// <remarks>This method uses keyset pagination to efficiently retrieve a subset of entities from the
    /// repository. The <paramref name="after"/> parameter is used to specify the starting point for the next page of
    /// results, and the <paramref name="orderBy"/> parameter determines the property used for ordering.  If <paramref
    /// name="orderBy"/> is not specified, the results are ordered by the entity's ID. The method also supports
    /// additional filtering through the <paramref name="predicate"/> and <paramref name="where"/> parameters.  The
    /// returned <see cref="PagingResultsList{T}"/> includes a cursor that can be used to fetch subsequent
    /// pages.</remarks>
    /// <typeparam name="T">The type of the entity being queried.</typeparam>
    /// <typeparam name="TKey">The type of the key used for pagination.</typeparam>
    /// <param name="repository">The repository from which to retrieve the entities.</param>
    /// <param name="predicate">An optional filter expression to apply to the query. If null, no filtering is applied.</param>
    /// <param name="keySelector">A function to select the key used for pagination from an entity.</param>
    /// <param name="orderBy">The name of the property to order the results by. If null or empty, results are ordered by the entity's ID.</param>
    /// <param name="after">An optional cursor indicating the starting point for the next page of results. If null, the first page is
    /// returned.</param>
    /// <param name="limit">The maximum number of entities to retrieve. Defaults to 20.</param>
    /// <param name="where">An optional array of additional filter conditions to apply to the query.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="PagingResultsList{T}"/> containing the retrieved entities, the total count of matching entities,
    /// and a cursor for retrieving the next page of results.</returns>
    public static async Task<PagingResultsList<T>> KeySetResult<T, TKey>(
        this IRepository<T, TKey> repository
        , Expression<Func<T, bool>>? predicate
        , Func<T, TKey> keySelector
        , string? orderBy
        , string? after = null
        , int limit = 20
        , string[]? where = null
        , CancellationToken cancellationToken = default
        )
        where T : IEntity<TKey>
        where TKey : notnull
    {
        var querySpecification = new Specification<T>();
        if (predicate is not null)
            querySpecification.Where(predicate);

        _ = PagingCursor<TKey>.TryParse(after, out PagingCursor<TKey>? afterCursor);

        if (!string.IsNullOrEmpty(orderBy))
        {
            querySpecification.OrderBy(orderBy);
        }

        if (afterCursor is not null && !string.IsNullOrEmpty(orderBy) && afterCursor.OrderByValue is not null)
        {
            querySpecification.Where(QueryableExtensions.KeysetPagePredicate<T>(
                (orderBy, (object)afterCursor.OrderByValue),
                ("ID", (object)afterCursor.DefaultValue)
            ));

        }
        else if (afterCursor is not null)
        {
            querySpecificiation.Where($"ID ge '{afterCursor.DefaultValue}'");
        }

        querySpecificiation
            .OrderBy(_ => _.Id)
            .Take(limit + 1) // last +1
            .WhereRange(where);

        var results = await repository
            .FindMany(querySpecificiation)
            .ToArrayAsync(cancellationToken);

        if (results.Length > 0)
        {
            var resultCount = await repository.Count(
                querySpecificiation.Clear().WhereRange(where).WhereIfNotNull(predicate)
                , cancellationToken);

            var lastItem = results.Last();
            var key1 = keySelector(lastItem);

            object? orderByValue = null;
            if (orderBy is not null)
            {
                // Use cached property info for performance
                var prop = _propertyCache.GetOrAdd(
                    (typeof(T), orderBy),
                    key => key.Item1.GetProperty(
                        key.Item2,
                        System.Reflection.BindingFlags.IgnoreCase |
                        System.Reflection.BindingFlags.Public |
                        System.Reflection.BindingFlags.Instance)
                );
                orderByValue = prop?.GetValue(lastItem);
            }

            var afterCursorValue = PagingCursor<TKey>.Create(key1, orderByValue).ToString();
            return new PagingResultsList<T>(results, resultCount, limit, afterCursorValue);
        }

        return PagingResultsList<T>.Empty();
    }
}

using Arcturus.Data.Abstracts;
using System.Linq.Expressions;

namespace Arcturus.Data.Repository.Abstracts;

/// <summary>
/// Defines a generic repository interface for managing entities of type <typeparamref name="T"/> with a primary key of
/// type <typeparamref name="TKey"/>.
/// </summary>
/// <remarks>This interface provides methods for common data access operations, such as querying, adding,
/// updating, and deleting entities. It supports asynchronous operations and is designed to work with LINQ expressions
/// for flexible querying.</remarks>
/// <typeparam name="T">The type of the entity managed by the repository. Must implement <see cref="IEntity{TKey}"/>.</typeparam>
/// <typeparam name="TKey">The type of the primary key for the entity. Must be non-nullable.</typeparam>
public interface IRepository<T, TKey> 
    where T : IEntity<TKey> where TKey : notnull
{
    /// <summary>
    /// Determines whether any elements in the data source satisfy the specified condition.
    /// </summary>
    /// <remarks>This method evaluates the condition specified by <paramref name="predicate"/> against each
    /// element  in the data source. The operation is canceled if <paramref name="cancellationToken"/> is
    /// triggered.</remarks>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains  <see langword="true"/> if any
    /// elements satisfy the condition specified by <paramref name="predicate"/>;  otherwise, <see langword="false"/>.</returns>
    ValueTask<bool> Any(
        Expression<Func<T, bool>> predicate
        , CancellationToken cancellationToken);
    /// <summary>
    /// Determines whether any elements in the data source satisfy the specified condition.
    /// <para>
    /// Void using take, skip, order by, and include expressions.
    /// </para>
    /// </summary>
    /// <param name="specification">A given <see cref="Specification{T}"/> to use for query.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains  <see langword="true"/> if any
    /// elements satisfy the condition specified by <paramref name="specification"/>;  otherwise, <see langword="false"/>.</returns>
    ValueTask<bool> Any(
        ISpecification<T> specification
        , CancellationToken cancellationToken);
    /// <summary>
    /// Asynchronously counts the number of entities that satisfy the specified predicate.
    /// </summary>
    /// <param name="predicate">An expression that defines the conditions the entities must satisfy to be included in the count.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests. Passing a canceled token will result in the task being canceled.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the count of entities that match the
    /// specified predicate.</returns>
    Task<long> Count(
        Expression<Func<T, bool>> predicate
        , CancellationToken cancellationToken);
    /// <summary>
    /// Asynchronously counts the number of entities that satisfy the specified predicate.
    /// <para>
    /// Void using take, skip, order by, and include expressions.
    /// </para>
    /// </summary>
    /// <param name="specification">Specification expression that defines the conditions.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests. Passing a canceled token will result in the task being canceled.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the count of entities that match the
    /// specified predicate.</returns>
    Task<long> Count(
        ISpecification<T> specification
        , CancellationToken cancellationToken);
    /// <summary>
    /// Adds the specified entity to the underlying data store asynchronously.
    /// </summary>
    /// <remarks>The entity is added to the data store, but the changes may not be persisted immediately
    /// depending on the implementation. Ensure that the operation is awaited to guarantee completion.</remarks>
    /// <param name="entity">The entity to add. Cannot be null.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests. If the operation is canceled, the task will complete with a
    /// canceled state.</param>
    /// <returns>A <see cref="ValueTask"/> that represents the asynchronous operation. The task completes when the entity has
    /// been added.</returns>
    ValueTask Add(T entity, CancellationToken cancellationToken);
    /// <summary>
    /// Deletes the specified entity from the data store.
    /// </summary>
    /// <remarks>This method removes the provided entity from the underlying data store.  Ensure that the
    /// entity exists in the data store before calling this method to avoid unexpected behavior.</remarks>
    /// <param name="entity">The entity to delete. Must not be <see langword="null"/>.</param>
    void Delete(T entity);
    /// <summary>
    /// Deletes the entity with the specified identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the entity to delete.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the number of entities deleted.</returns>
    Task<int> Delete(TKey id, CancellationToken cancellationToken);
    /// <summary>
    /// Retrieves a sequence of items from the data source that match the specified predicate,  projects them into a new
    /// form, and optionally applies ordering, limits, and tracking behavior.
    /// </summary>
    /// <remarks>This method is designed for scenarios where large result sets may be processed incrementally 
    /// using asynchronous enumeration. Ensure proper disposal of the enumerator to release resources.</remarks>
    /// <typeparam name="R">The type of the result elements after projection.</typeparam>
    /// <param name="predicate">An expression that defines the conditions each item must satisfy to be included in the result set.</param>
    /// <param name="select">An expression that specifies how to project each item in the result set into the desired form.</param>
    /// <param name="take">An optional parameter that specifies the maximum number of items to return. If null, no limit is applied.</param>
    /// <param name="orderBy">An optional expression that specifies the property by which to order the result set. If null, no ordering is
    /// applied.</param>
    /// <param name="tracking">An optional parameter that specifies whether the returned entities should be tracked by the underlying context. 
    /// If <see langword="true"/>, tracking is enabled; otherwise, tracking is disabled. Defaults to <see
    /// langword="false"/>.</param>
    /// <returns>An asynchronous stream of projected items that match the specified conditions.</returns>
    IAsyncEnumerable<R> FindMany<R>(
        Expression<Func<T, bool>> predicate
        , Expression<Func<T, R>> select
        , int? take = null
        , Expression<Func<T, object>>? orderBy = null
        , bool? tracking = false);
    /// <summary>
    /// Asynchronously retrieves a single entity that matches the specified predicate.
    /// </summary>
    /// <remarks>Use this method to retrieve a single entity based on a condition. If multiple entities match
    /// the predicate, only the first one will be returned. If no entity matches, the result will be <see
    /// langword="null"/>.</remarks>
    /// <param name="predicate">An expression that defines the condition the entity must satisfy.</param>
    /// <param name="tracking">A boolean value indicating whether the retrieved entity should be tracked by the context. If <see
    /// langword="true"/>, the entity will be tracked; otherwise, it will not be tracked. The default is <see
    /// langword="false"/>.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the entity that matches the
    /// predicate, or <see langword="null"/> if no such entity is found.</returns>
    Task<T?> FindOne(
        Expression<Func<T, bool>> predicate
        , bool tracking = false
        , CancellationToken cancellationToken = default);
    /// <summary>
    /// Finds a single entity that matches the specified predicate and projects it to the specified result type.
    /// </summary>
    /// <remarks>If multiple entities match the predicate, only the first one is returned. The behavior of
    /// this method  depends on the underlying data source and query provider.</remarks>
    /// <typeparam name="R">The type of the result to project the entity to.</typeparam>
    /// <param name="predicate">An expression that defines the condition the entity must satisfy.</param>
    /// <param name="select">An expression that specifies how to project the entity to the result type.</param>
    /// <param name="tracking">A value indicating whether the entity should be tracked by the context.  <see langword="true"/> to enable
    /// tracking; otherwise, <see langword="false"/>.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the projected entity  of type
    /// <typeparamref name="R"/>, or <see langword="null"/> if no entity matches the predicate.</returns>
    Task<R?> FindOne<R>(
        Expression<Func<T, bool>> predicate
        , Expression<Func<T, R>> select
        , bool tracking = false
        , CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds a single entity that matches the specified predicate and projects it to the specified result type.
    /// </summary>
    /// <param name="specification">A given <see cref="Specification{T}"/> to use for query.</param>
    /// <param name="tracking">A value indicating whether the entity should be tracked by the context.  <see langword="true"/> to enable
    /// tracking; otherwise, <see langword="false"/>.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the projected entity  of type
    /// <typeparamref name="T"/>, or <see langword="null"/> if no entity matches the predicate.</returns>
    /// <exception cref="NotImplementedException" />
    Task<T?> FindOne(
        ISpecification<T> specification
        , bool tracking = false
        , CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds a single entity that matches the specified predicate and projects it to the specified result type.
    /// </summary>
    /// <param name="specification">A given <see cref="Specification{T}"/> to use for query.</param>
    /// <param name="tracking">A value indicating whether the entity should be tracked by the context.  <see langword="true"/> to enable
    /// tracking; otherwise, <see langword="false"/>.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <typeparam name="TResult">The type of the result to project the entity to.</typeparam>
    /// <returns>A task that represents the asynchronous operation. The task result contains the projected entity  of type
    /// <typeparamref name="T"/>, or <see langword="null"/> if no entity matches the predicate.</returns>
    /// <exception cref="NotImplementedException" />
    Task<TResult?> FindOne<TResult>(
        ISpecification<T, TResult> specification
        , bool tracking = false
        , CancellationToken cancellationToken = default);
    /// <summary>
    /// Retrieves a sequence of items from the data source that match the specified predicate,  projects them into a new
    /// form, and optionally applies ordering, limits, and tracking behavior.
    /// </summary>
    /// <remarks>This method is designed for scenarios where large result sets may be processed incrementally 
    /// using asynchronous enumeration. Ensure proper disposal of the enumerator to release resources.</remarks>
    /// <param name="specification">A given <see cref="Specification{T}"/> to use for query.</param>
    /// <param name="tracking">An optional parameter that specifies whether the returned entities should be tracked by the underlying context. 
    /// If <see langword="true"/>, tracking is enabled; otherwise, tracking is disabled. Defaults to <see
    /// langword="false"/>.</param>
    /// <returns>An asynchronous stream of projected items that match the specified conditions.</returns>
    IAsyncEnumerable<T> FindMany(
        ISpecification<T> specification
        , bool tracking = false);
    /// <summary>
    /// Retrieves a sequence of items from the data source that match the specified predicate,  projects them into a new
    /// form, and optionally applies ordering, limits, and tracking behavior.
    /// </summary>
    /// <param name="specification">A given <see cref="Specification{T}"/> to use for query.</param>
    /// <param name="tracking">An optional parameter that specifies whether the returned entities should be tracked by the underlying context. 
    /// If <see langword="true"/>, tracking is enabled; otherwise, tracking is disabled. Defaults to <see
    /// langword="false"/>.</param>
    /// <remarks>This method is designed for scenarios where large result sets may be processed incrementally 
    /// using asynchronous enumeration. Ensure proper disposal of the enumerator to release resources.</remarks>
    /// <returns>An asynchronous stream of projected items that match the specified conditions.</returns>
    IAsyncEnumerable<TResult> FindMany<TResult>(
        ISpecification<T, TResult> specification
        , bool tracking = false);
}
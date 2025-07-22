using Arcturus.Repository.Abstracts;
using Arcturus.Repository.EntityFrameworkCore.Internals;
using Arcturus.Repository.Specification;

namespace Arcturus.Repository.EntityFrameworkCore;

/// <summary>
/// Provides a generic repository implementation for managing entities in a database context.
/// </summary>
/// <remarks>This repository provides common data access operations such as querying, adding, updating, and
/// deleting entities. It is designed to work with an <see cref="DbContext"/> and supports asynchronous operations for
/// improved scalability.</remarks>
/// <typeparam name="T">The type of the entity managed by the repository. Must be a class implementing <see cref="IEntity{TKey}"/>.</typeparam>
/// <typeparam name="TKey">The type of the primary key for the entity. Must be non-nullable.</typeparam>
/// <param name="context"></param>
public class Repository<T, TKey>(
    DbContext context) : IRepository<T, TKey>
    where T : class, IEntity<TKey>
    where TKey : notnull
{
    private readonly DbContext _context = context;

    public async ValueTask<bool> Any(
        Expression<Func<T, bool>> predicate
        , CancellationToken cancellationToken)
    {
        return await _context.Set<T>()
            .AsNoTracking()
            .AnyAsync(predicate, cancellationToken);
    }
    public async Task<long> Count(
        Expression<Func<T, bool>> predicate
        , CancellationToken cancellationToken)
    {
        return await _context.Set<T>()
            .AsNoTracking()
            .LongCountAsync(predicate, cancellationToken);
    }

    public void Delete(T entity) => _context.Set<T>().Remove(entity);

    public async Task<int> Delete(TKey id, CancellationToken cancellationToken)
    {
        return await _context.Set<T>()
            .Where(_ => _.Id.Equals(id))
            .ExecuteDeleteAsync(cancellationToken);
    }
    public async ValueTask Add(T entity, CancellationToken cancellationToken)
    {
        await _context.Set<T>().AddAsync(entity, cancellationToken);
    }

    public IAsyncEnumerable<R> FindMany<R>(
        Expression<Func<T, bool>> predicate
        , Expression<Func<T, R>> select
        , int? take = null
        , Expression<Func<T, object>>? orderBy = null
        , bool? tracking = false)
    {
        var query = _context
            .Set<T>()
            .AsQueryable();
        if (tracking == true)
            query = query.AsTracking();
        else
            query = query.AsNoTracking();

        query = query.Where(predicate);

        if (take is not null)
            query = query.Take(take.Value);

        query = query.OrderBy(orderBy ?? (_ => _.Id));

        return query
            .Select(select)
            .AsAsyncEnumerable();
    }

    public async Task<R?> FindOne<R>(
        Expression<Func<T, bool>> predicate
        , Expression<Func<T, R>> select
        , bool tracking = false
        , CancellationToken cancellationToken = default)
    {
        var query = _context
            .Set<T>()
            .AsQueryable();
        if (tracking)
            query = query
                .Where(predicate)
                .AsTracking();
        else
            query = query
                .Where(predicate)
                .AsNoTracking();

        return await query
            .Select(select)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<T?> FindOne(
        Expression<Func<T, bool>> predicate
        , bool tracking = false
        , CancellationToken cancellationToken = default)
    {
        if (tracking)
            return await _context.Set<T>()
                .Where(predicate)
                .AsTracking()
                .SingleOrDefaultAsync(cancellationToken);

        return await _context.Set<T>()
            .Where(predicate)
            .SingleOrDefaultAsync(cancellationToken);
    }
    public async Task<T?> FindOne(
        ISpecification<T> specification
        , bool tracking = false
        , CancellationToken cancellationToken = default)
    {
        var exec = new SqlSpecificationEvaluator<T>(specification);
        var query = exec.Apply(_context.Set<T>());
        if (tracking)
            query = query.AsTracking();

        return await query.SingleOrDefaultAsync(cancellationToken);
    }
    public async Task<TResult?> FindOne<TResult>(
        ISpecification<T, TResult> specification
        , bool tracking = false
        , CancellationToken cancellationToken = default)
    {
        var exec = new SqlSpecificationEvaluator<T>(specification);
        var set = _context.Set<T>().AsQueryable();
        if (tracking)
            set = set.AsTracking();

        var query = exec.Apply<TResult>(set);

        return await query.SingleOrDefaultAsync(cancellationToken);
    }

    public IAsyncEnumerable<T> FindMany(
        ISpecification<T> specification
        , bool tracking = false)
    {
        var exec = new SqlSpecificationEvaluator<T>((Specification<T>)specification);
        var query = exec.Apply(_context.Set<T>());
        if (tracking)
            query = query.AsTracking();

        return query.AsAsyncEnumerable();
    }

    public IAsyncEnumerable<TResult> FindMany<TResult>(
        ISpecification<T, TResult> specification
        , bool tracking = false)
    {
        var exec = new SqlSpecificationEvaluator<T>((Specification<T, TResult>)specification);
        var set = _context.Set<T>().AsQueryable();
        if (tracking)
            set = set.AsTracking();

        var query = exec.Apply<TResult>(set);
        
        return query.AsAsyncEnumerable();
    }
    public async Task<long> Count(
        ISpecification<T> specification
        , CancellationToken cancellationToken)
    {
        var exec = new SqlSpecificationEvaluator<T>((Specification<T>)specification);
        var query = exec.Apply(_context.Set<T>());

        return await query
            .AsNoTracking()
            .LongCountAsync(cancellationToken);
    }
    public async ValueTask<bool> Any(
        ISpecification<T> specification
        , CancellationToken cancellationToken)
    {
        var exec = new SqlSpecificationEvaluator<T>((Specification<T>)specification);
        var query = exec.Apply(_context.Set<T>());

        return await query
            .AsNoTracking()
            .AnyAsync(cancellationToken);
    }
}
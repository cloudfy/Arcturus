using Arcturus.Repository.Abstracts;
using Arcturus.Repository.EntityFrameworkCore.Internals;
using Arcturus.Repository.Specification;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Arcturus.Repository.EntityFrameworkCore.PostgresSql;

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

    public Task<AddOrUpdateResult<T>> AddOrUpdate(
        T value
        , Expression<Func<T, bool>> predicate
        , Expression<Func<T, object>>? updateColumns = null
        , CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("AddOrUpdate is not implemented for PostgreSQL provider.");
        //return EfAddOrUpdateExtensions.AddOrUpdateWithResult<T>(
        //    _context.Set<T>(), value, predicate, updateColumns, cancellationToken);
    }
}
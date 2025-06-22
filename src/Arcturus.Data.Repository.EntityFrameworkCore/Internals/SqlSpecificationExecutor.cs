using Arcturus.Data.Repository.Abstracts;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;

namespace Arcturus.Data.Repository.EntityFrameworkCore.Internals;

internal sealed class SqlSpecificationExecutor<TEntity>(Specification<TEntity> specification) 
    : SpecificationExecutor<TEntity>(specification)
    where TEntity : class
{
    private readonly Specification<TEntity> _specification = specification;

    public override IQueryable<TEntity> Apply(IQueryable<TEntity> source)
    {
        foreach (var chain in _specification.IncludeChains)
        {
            source = ApplyIncludeChain(source, chain);
        }
        if (_specification.UseSplitQuery)
        {
            source = source.AsSplitQuery();
        }
        // Apply OrderBy/ThenBy if specified
        bool ordered = false;
        foreach (var (expr, descending) in _specification.OrderBy)
        {
            if (expr is LambdaExpression lambda)
            {
                var methodName = !ordered
                    ? (descending ? "OrderByDescending" : "OrderBy")
                    : (descending ? "ThenByDescending" : "ThenBy");

                source = ApplyOrderBy(source, lambda, methodName);
                ordered = true;
            }
        }
        return source.Where(_specification.Predicate);
    }
    private static IQueryable<TEntity> ApplyOrderBy(IQueryable<TEntity> source, LambdaExpression keySelector, string methodName)
    {
        var entityType = typeof(TEntity);
        var keyType = keySelector.Body.Type;
        var method = typeof(Queryable).GetMethods(BindingFlags.Public | BindingFlags.Static)
            .First(m => m.Name == methodName
                        && m.GetParameters().Length == 2);

        var genericMethod = method.MakeGenericMethod(entityType, keyType);
        return (IQueryable<TEntity>)genericMethod.Invoke(null, [source, keySelector])!;
    }
    private static IQueryable<TEntity> ApplyIncludeChain(IQueryable<TEntity> source, List<LambdaExpression> chain)
    {
        if (chain.Count == 0) return source;

        var query = EntityFrameworkQueryableExtensions.Include(source, (dynamic)chain[0]);

        for (int i = 1; i < chain.Count; i++)
        {
            query = EntityFrameworkQueryableExtensions.ThenInclude((dynamic)query, (dynamic)chain[i]);
        }

        return query;
    }
}
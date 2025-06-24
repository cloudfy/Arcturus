using Arcturus.Data.Repository.Abstracts;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Arcturus.Data.Repository.EntityFrameworkCore.Internals;

internal sealed class SqlSpecificationEvaluator<TEntity>(Specification<TEntity> specification) 
    : SpecificationEvaluator<TEntity>(specification)
    where TEntity : class
{
    public override IQueryable<TEntity> Apply(IQueryable<TEntity> source)
    {
        foreach (var chain in Specification.IncludeChains)
        {
            source = ApplyIncludeChain(source, chain);
        }
        if (Specification.UseSplitQuery)
        {
            source = source.AsSplitQuery();
        }
        return ApplyWhere(ApplyTake(ApplyOrderBy(source)));
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
using Arcturus.Data.Repository.Abstracts;
using Microsoft.EntityFrameworkCore;

namespace Arcturus.Data.Repository.EntityFrameworkCore.Internals;

internal sealed class SqlSpecificationExecutor<TEntity>(Specification<TEntity> specification) 
    : SpecificationExecutor<TEntity>(specification)
    where TEntity : class
{
    public override IQueryable<TEntity> Apply(IQueryable<TEntity> source)
    {
        foreach (var include in Specification.Includes)
        {
            source = source.AsQueryable().Include(include);
        }

        return source.Where(Specification.Predicate);
    }
}
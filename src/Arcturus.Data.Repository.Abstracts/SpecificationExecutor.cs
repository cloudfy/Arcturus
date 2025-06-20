namespace Arcturus.Data.Repository.Abstracts;

public abstract class SpecificationExecutor<TEntity>(Specification<TEntity> specification)
    where TEntity : class
{
    protected readonly Specification<TEntity> Specification = specification;

    public abstract IQueryable<TEntity> Apply(IQueryable<TEntity> source);
}
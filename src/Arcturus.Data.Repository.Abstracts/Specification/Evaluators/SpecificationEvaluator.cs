namespace Arcturus.Repository.Specification;

/// <summary>
/// Provides a base class for executing specifications on a queryable source.
/// </summary>
/// <remarks>This class is designed to encapsulate the logic for applying a specification to a queryable source.
/// Derived classes must implement the <see cref="Apply"/> method to define the specific query transformation based on
/// the provided <see cref="Specification{TEntity}"/>.</remarks>
/// <typeparam name="TEntity">The type of the entity being queried. Must be a reference type.</typeparam>
/// <param name="specification"></param>
public abstract class SpecificationEvaluator<TEntity>(ISpecification<TEntity> specification)
    where TEntity : class
{
    protected internal ISpecification<TEntity> Specification = specification;

    /// <summary>
    /// Applies a query transformation to the specified <see cref="IQueryable{TEntity}"/> source.
    /// </summary>
    /// <remarks>This method is intended to modify or filter the provided query source based on specific
    /// criteria or logic. The exact transformation depends on the implementation in derived classes.</remarks>
    /// <param name="source">The <see cref="IQueryable{TEntity}"/> to which the transformation will be applied. Cannot be null.</param>
    /// <returns>A transformed <see cref="IQueryable{TEntity}"/> representing the result of the applied query.</returns>
    public abstract IQueryable<TEntity> Apply(IQueryable<TEntity> source);
    /// <summary>
    /// Applies a query transformation to the specified <see cref="IQueryable{TEntity}"/> source.
    /// </summary>
    /// <remarks>This method is intended to modify or filter the provided query source based on specific
    /// criteria or logic. The exact transformation depends on the implementation in derived classes.</remarks>
    /// <param name="source">The <see cref="IQueryable{TEntity}"/> to which the transformation will be applied. Cannot be null.</param>
    /// <returns>A transformed <see cref="IQueryable{TResult}"/> representing the result of the applied query.</returns>
    public abstract IQueryable<TResult> Apply<TResult>(IQueryable<TEntity> source);

    protected internal IQueryable<TEntity> ApplyLimit(IQueryable<TEntity> source)
    {
        if (Specification.Limit is not null)
        {
            source = source.Take(Specification.Limit.Value);
        }
        return source;
    }
    protected internal IQueryable<TEntity> ApplySkip(IQueryable<TEntity> source)
    {
        if (Specification.Skip is not null)
        {
            source = source.Skip(Specification.Skip.Value);
        }
        return source;
    }
    protected internal IQueryable<TEntity> ApplyWhere(IQueryable<TEntity> source)
    {
        foreach (var where in Specification.WhereExpressions)
        {
            source = source.Where(where.Filter);
        }
        return source;
    }
    protected internal IQueryable<TEntity> ApplyOrderBy(IQueryable<TEntity> source)
    {
        IOrderedQueryable<TEntity>? orderedQuery = null;
        bool ordered = false;
        foreach (var orderExpression in Specification.OrderExpressions)
        {
            if (ordered & orderExpression.Descending)
            {
                orderedQuery = orderedQuery!.ThenByDescending(orderExpression.OrderBy);
            }
            else if (ordered & !orderExpression.Descending)
            {
                orderedQuery = orderedQuery!.ThenBy(orderExpression.OrderBy);
            }
            else if (!ordered & orderExpression.Descending)
            {
                orderedQuery = orderedQuery!.OrderByDescending(orderExpression.OrderBy);
            }
            else
            {
                orderedQuery = source.OrderBy(orderExpression.OrderBy);
            }

            ordered = true;
        }
        if (orderedQuery is not null)
        {
            return orderedQuery;
        }

        return source;
    }
}
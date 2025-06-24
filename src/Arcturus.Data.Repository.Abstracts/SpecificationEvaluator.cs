using System.Linq.Expressions;
using System.Reflection;

namespace Arcturus.Data.Repository.Abstracts;

/// <summary>
/// Provides a base class for executing specifications on a queryable source.
/// </summary>
/// <remarks>This class is designed to encapsulate the logic for applying a specification to a queryable source.
/// Derived classes must implement the <see cref="Apply"/> method to define the specific query transformation based on
/// the provided <see cref="Specification{TEntity}"/>.</remarks>
/// <typeparam name="TEntity">The type of the entity being queried. Must be a reference type.</typeparam>
/// <param name="specification"></param>
public abstract class SpecificationEvaluator<TEntity>(Specification<TEntity> specification)
    where TEntity : class
{
    protected internal Specification<TEntity> Specification = specification;

    /// <summary>
    /// Applies a query transformation to the specified <see cref="IQueryable{TEntity}"/> source.
    /// </summary>
    /// <remarks>This method is intended to modify or filter the provided query source based on specific
    /// criteria or logic. The exact transformation depends on the implementation in derived classes.</remarks>
    /// <param name="source">The <see cref="IQueryable{TEntity}"/> to which the transformation will be applied. Cannot be null.</param>
    /// <returns>A transformed <see cref="IQueryable{TEntity}"/> representing the result of the applied query.</returns>
    public abstract IQueryable<TEntity> Apply(IQueryable<TEntity> source);

    /// <summary>
    /// Applies a limit to the number of elements in the queryable sequence based on the specification.
    /// </summary>
    /// <remarks>This method checks the <see cref="Specification.Limit"/> property and applies the <see
    /// cref="Queryable.Take(int)"/>  method to restrict the number of elements in the sequence. If the limit is not
    /// set, the method returns the  original sequence without modification.</remarks>
    /// <param name="source">The source queryable sequence to which the limit will be applied.</param>
    /// <returns>A queryable sequence containing at most the number of elements specified by the limit in the specification. If
    /// no limit is specified, the original sequence is returned unchanged.</returns>
    protected internal IQueryable<TEntity> ApplyTake(IQueryable<TEntity> source)
    {
        if (Specification.Limit is not null)
        {
            source = source.Take(Specification.Limit.Value);
        }
        return source;
    }
    /// <summary>
    /// Applies the filtering criteria defined in the specification to the provided queryable source.
    /// </summary>
    /// <remarks>This method iterates through the filtering expressions defined in the specification and
    /// applies them sequentially to the provided queryable source using the <see cref="Queryable.Where"/> method. The
    /// resulting queryable reflects all specified filtering conditions.</remarks>
    /// <param name="source">The <see cref="IQueryable{TEntity}"/> to which the filtering criteria will be applied.</param>
    /// <returns>A new <see cref="IQueryable{TEntity}"/> that includes the filtering conditions specified in the current
    /// specification.</returns>
    protected internal IQueryable<TEntity> ApplyWhere(IQueryable<TEntity> source) 
    {
        foreach (var where in Specification.Where)
        {
            source = source.Where(where);
        }
        return source;
    }
    /// <summary>
    /// Applies the ordering specified in the <see cref="Specification.OrderBy"/> collection to the given queryable
    /// source.
    /// </summary>
    /// <remarks>This method processes the <see cref="Specification.OrderBy"/> collection, which contains
    /// expressions and their corresponding sort directions, and applies them sequentially using the appropriate LINQ
    /// methods (<c>OrderBy</c>, <c>OrderByDescending</c>, <c>ThenBy</c>, or <c>ThenByDescending</c>).</remarks>
    /// <param name="source">The <see cref="IQueryable{TEntity}"/> to which the ordering will be applied.</param>
    /// <returns>A new <see cref="IQueryable{TEntity}"/> with the specified ordering applied. If no ordering is specified, the
    /// original <paramref name="source"/> is returned unchanged.</returns>
    protected internal IQueryable<TEntity> ApplyOrderBy(IQueryable<TEntity> source)
    {
        // Apply OrderBy/ThenBy if specified
        bool ordered = false;
        foreach (var (expr, descending) in Specification.OrderBy)
        {
            if (expr is LambdaExpression lambda)
            {
                var methodName = !ordered
                    ? (descending ? "OrderByDescending" : "OrderBy")
                    : (descending ? "ThenByDescending" : "ThenBy");

                source = ExecuteOrderBy(source, lambda, methodName);
                ordered = true;
            }
        }
        return source;
    }
    private static IQueryable<TEntity> ExecuteOrderBy(
        IQueryable<TEntity> source, LambdaExpression keySelector, string methodName)
    {
        var entityType = typeof(TEntity);
        var keyType = keySelector.Body.Type;
        var method = typeof(Queryable)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .First(m => m.Name == methodName && m.GetParameters().Length == 2);

        var genericMethod = method.MakeGenericMethod(entityType, keyType);
        return (IQueryable<TEntity>)genericMethod.Invoke(null, [source, keySelector])!;
    }
}
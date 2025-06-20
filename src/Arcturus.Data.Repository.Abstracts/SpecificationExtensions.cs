using System.Linq.Expressions;

namespace Arcturus.Data.Repository.Abstracts;

public static class SpecificationExtensions
{
    /// <summary>
    /// Adds a navigation property to the specification, enabling eager loading of related entities.
    /// </summary>
    /// <remarks>Use this method to specify related entities that should be included in the query results.
    /// This is particularly useful for scenarios where lazy loading is disabled or when optimizing performance by
    /// reducing the number of database queries.</remarks>
    /// <typeparam name="TEntity">The type of the entity being queried.</typeparam>
    /// <typeparam name="TProperty">The type of the navigation property to include.</typeparam>
    /// <param name="spec">The specification to which the navigation property will be added.</param>
    /// <param name="navigationPropertyPath">An expression representing the navigation property to include. For example, <c>x => x.Property</c>.</param>
    /// <returns>An <see cref="IncludableSpecificationBuilder{TEntity, TProperty}"/> that allows chaining additional
    /// configuration for the included navigation property.</returns>
    public static IncludableSpecificationBuilder<TEntity, TProperty> Include<TEntity, TProperty>(
        this Specification<TEntity> spec,
        Expression<Func<TEntity, TProperty>> navigationPropertyPath)
        where TEntity : class
    {
        var includeAsObject = Expression.Lambda<Func<TEntity, object>>(
            Expression.Convert(navigationPropertyPath.Body, typeof(object)),
            navigationPropertyPath.Parameters);

        spec.AddInclude(includeAsObject);
        return new IncludableSpecificationBuilder<TEntity, TProperty>(includeAsObject);
    }

    public static IncludableSpecificationBuilder<TEntity, TNextProperty> ThenInclude<TEntity, TPreviousProperty, TNextProperty>(
        this IncludableSpecificationBuilder<TEntity, IEnumerable<TPreviousProperty>> source,
        Expression<Func<TPreviousProperty, TNextProperty>> navigationPropertyPath)
    {
        // Optionally track the include chain if needed
        return new IncludableSpecificationBuilder<TEntity, TNextProperty>(source.CurrentInclude);
    }

    public static IncludableSpecificationBuilder<TEntity, TNextProperty> ThenInclude<TEntity, TPreviousProperty, TNextProperty>(
        this IncludableSpecificationBuilder<TEntity, TPreviousProperty> source,
        Expression<Func<TPreviousProperty, TNextProperty>> navigationPropertyPath)
    {
        return new IncludableSpecificationBuilder<TEntity, TNextProperty>(source.CurrentInclude);
    }

    private static Expression<Func<T, bool>> AsQueryable<T>(this Func<T, bool> predicate)
    {
        var param = Expression.Parameter(typeof(T));
        var body = Expression.Invoke(Expression.Constant(predicate), param);
        return Expression.Lambda<Func<T, bool>>(body, param);
    }
}

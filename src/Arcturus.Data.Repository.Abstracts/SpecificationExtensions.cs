using System.Linq.Expressions;

namespace Arcturus.Data.Repository.Abstracts;

public static class SpecificationExtensions
{
    /// <summary>
    /// Limits the number of entities returned by the specification.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity the specification applies to.</typeparam>
    /// <param name="specification">The specification to apply the limit to.</param>
    /// <param name="take">The maximum number of entities to return. Must be greater than or equal to zero.</param>
    /// <returns>The updated specification with the limit applied.</returns>
    public static Specification<TEntity> Take<TEntity>(
        this Specification<TEntity> specification
        , int take) where TEntity : class
    {
        specification.Limit = take;
        return specification;
    }
    /// <summary>
    /// Configures the specification to use split queries when executing database operations.
    /// </summary>
    /// <remarks>Split queries are used to execute database operations in multiple queries, which can improve
    /// performance  in certain scenarios, such as when working with large data sets or complex relationships.  This
    /// method modifies the provided specification to enable split query behavior.</remarks>
    /// <typeparam name="TEntity">The type of the entity being queried.</typeparam>
    /// <param name="specification">The specification to configure. Cannot be <see langword="null"/>.</param>
    /// <returns>The modified specification with split query behavior enabled.</returns>
    public static Specification<TEntity> AsSplitQuery<TEntity>(
        this Specification<TEntity> specification)
    {
        specification.UseSplitQuery = true;
        return specification;
    }
    /// <summary>
    /// Adds a filtering condition to the current specification based on the provided predicate.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity being filtered.</typeparam>
    /// <param name="specification">The specification to which the filtering condition will be added.</param>
    /// <param name="predicate">An expression that defines the filtering condition for the entity.</param>
    /// <returns>The updated specification with the added filtering condition.</returns>
    public static Specification<TEntity> Where<TEntity>(
        this Specification<TEntity> specification
        , Expression<Func<TEntity, bool>> predicate)
    {
        specification.AddWhere(predicate);
        return specification;
    }

    public static IncludableSpecificationBuilder<TEntity, TProperty> Include<TEntity, TProperty>(
        this Specification<TEntity> spec,
        Expression<Func<TEntity, TProperty>> navigationPropertyPath)
        where TEntity : class
    {
        var builder = new IncludableSpecificationBuilder<TEntity, TProperty>(navigationPropertyPath);
        spec.AddIncludeChain(builder.IncludeChain);
        return builder;
    }

    public static IncludableSpecificationBuilder<TEntity, TNextProperty> ThenInclude<TEntity, TPreviousProperty, TNextProperty>(
        this IncludableSpecificationBuilder<TEntity, IEnumerable<TPreviousProperty>> source,
        Expression<Func<TPreviousProperty, TNextProperty>> navigationPropertyPath)
    {
        return new IncludableSpecificationBuilder<TEntity, TNextProperty>(source.IncludeChain, navigationPropertyPath);
    }

    public static IncludableSpecificationBuilder<TEntity, TNextProperty> ThenInclude<TEntity, TPreviousProperty, TNextProperty>(
        this IncludableSpecificationBuilder<TEntity, TPreviousProperty> source,
        Expression<Func<TPreviousProperty, TNextProperty>> navigationPropertyPath)
    {
        return new IncludableSpecificationBuilder<TEntity, TNextProperty>(source.IncludeChain, navigationPropertyPath);
    }

}


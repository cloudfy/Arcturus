    using System.Linq.Expressions;

namespace Arcturus.Data.Repository.Abstracts;

public static class SpecificationExtensions
{
    /// <summary>
    /// Limits the number of entities returned by the specification.
    /// </summary>
    /// <param name="take">The maximum number of entities to return. Must be greater than or equal to zero.</param>
    /// <returns>The updated specification with the limit applied.</returns>
    public static Specification<T> Take<T>(this Specification<T> spec, int take)
        => spec.InnerTake(take);
    /// <summary>
    /// Limits the number of entities returned by the specification.
    /// </summary>
    /// <param name="take">The maximum number of entities to return. Must be greater than or equal to zero.</param>
    /// <returns>The updated specification with the limit applied.</returns>
    public static Specification<T, R> Take<T, R>(this Specification<T, R> spec, int take)
        => (Specification<T, R>)spec.InnerTake(take);

    /// <summary>
    /// Adds a filtering condition to the current specification based on the provided predicate.
    /// </summary>
    /// <param name="predicate">An expression that defines the filtering condition for the entity.</param>
    /// <returns>The updated specification with the added filtering condition.</returns>
    public static Specification<T> Where<T>(this Specification<T> spec, Expression<Func<T, bool>> predicate)
        => spec.InnerWhere(predicate);
    /// <summary>
    /// Adds a filtering condition to the current specification based on the provided predicate.
    /// </summary>
    /// <param name="predicate">An expression that defines the filtering condition for the entity.</param>
    /// <returns>The updated specification with the added filtering condition.</returns>
    public static Specification<T, R> Where<T, R>(this Specification<T, R> spec, Expression<Func<T, bool>> predicate)
        => (Specification<T, R>)spec.InnerWhere(predicate);
    /// <summary>
    /// Adds an order by condition to the current specification.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="spec"></param>
    /// <param name="orderByExpression">Order by expression.</param>
    /// <param name="descending">True to order by descending. Otherwise ascending.</param>
    /// <returns><see cref="Specification{T}"/></returns>
    public static Specification<T> OrderBy<T>(
        this Specification<T> spec
        , Expression<Func<T, object?>> orderByExpression, bool descending = false)
        => spec.InnerOrderBy(orderByExpression, descending);
    /// <summary>
    /// Adds an order by condition to the current specification.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="spec"></param>
    /// <param name="orderByExpression">Order by expression.</param>
    /// <param name="descending">True to order by descending. Otherwise ascending.</param>
    /// <returns><see cref="Specification{T, TResult}"/></returns>
    public static Specification<T, R> OrderBy<T, R>(
        this Specification<T, R> spec
        , Expression<Func<T, object?>> orderByExpression, bool descending = false)
        => (Specification<T, R>)spec.InnerOrderBy(orderByExpression, descending);

    /// <summary>
    /// Projects the <typeparamref name="T"/> to <typeparamref name="R"/> based on the <paramref name="selector"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="R"></typeparam>
    /// <param name="spec"></param>
    /// <param name="selector"></param>
    /// <returns><see cref="Specification{T, TResult}"/></returns>
    public static Specification<T, R> Project<T, R>(this Specification<T, R> spec, Expression<Func<T, R>> selector)
        => spec.InnerProject(selector);

    /// <summary>
    /// Configures the specification to use split queries when executing database operations.
    /// </summary>
    /// <remarks>Split queries are used to execute database operations in multiple queries, which can improve
    /// performance  in certain scenarios, such as when working with large data sets or complex relationships.  This
    /// method modifies the provided specification to enable split query behavior.</remarks>
    /// <returns>The modified specification with split query behavior enabled.</returns>
    public static Specification<T> WithSplitQuery<T>(this Specification<T> spec, bool value = true)
        => spec.InnerWithSplitQuery(value);
    /// <summary>
    /// Configures the specification to use split queries when executing database operations.
    /// </summary>
    /// <remarks>Split queries are used to execute database operations in multiple queries, which can improve
    /// performance  in certain scenarios, such as when working with large data sets or complex relationships.  This
    /// method modifies the provided specification to enable split query behavior.</remarks>
    /// <returns>The modified specification with split query behavior enabled.</returns>
    public static Specification<T, R> WithSplitQuery<T, R>(this Specification<T, R> spec, bool value = true)
        => (Specification<T, R>)spec.InnerWithSplitQuery(value);
    /// <summary>
    /// Configures the specification to ignore query filters.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="spec"></param>
    /// <param name="value">If true, query filters are ignored.</param>
    /// <returns>The modified specification with query filters configured.</returns>
    public static Specification<T> WithIgnoreQueryFilters<T>(this Specification<T> spec, bool value = true)
        => spec.InnerWithIgnoreQueryFilters(value);
    /// <summary>
    /// Configures the specification to ignore query filters.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="R"></typeparam>
    /// <param name="spec"></param>
    /// <param name="value">If true, query filters are ignored.</param>
    /// <returns>The modified specification with query filters configured.</returns>
    public static Specification<T, R> WithIgnoreQueryFilters<T, R>(this Specification<T, R> spec, bool value = true)
            => (Specification<T, R>)spec.InnerWithIgnoreQueryFilters(value);

    /// <summary>
    /// Converts <paramref name="source"/> to a projectable <see cref="ISpecification{TEntity, TResult}"/>.
    /// </summary>
    /// <typeparam name="T">Source type.</typeparam>
    /// <typeparam name="R">Target type.</typeparam>
    /// <param name="source">Source of <see cref="Specification{T}"/>.</param>
    /// <param name="selector">Expression to project.</param>
    /// <returns><see cref="Specification{T, TResult}"/>.</returns>
    public static Specification<T, R> WithProjection<T, R>(
        this Specification<T> source
        , Expression<Func<T, R>> selector)
    {
        var newSpec = source.Clone<R>();
        newSpec.Project(selector);
        return newSpec;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TProperty"></typeparam>
    /// <param name="spec"></param>
    /// <param name="navigationPropertyPath"></param>
    /// <returns></returns>
    public static IncludableSpecificationBuilder<TEntity, TProperty> Include<TEntity, TProperty>(
        this Specification<TEntity> spec,
        Expression<Func<TEntity, TProperty>> navigationPropertyPath)
        where TEntity : class
    {
        var builder = new IncludableSpecificationBuilder<TEntity, TProperty>(navigationPropertyPath);
        spec.Add(new Specification.Expressions.IncludeExpression(builder.IncludeChain));
        return builder;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TPreviousProperty"></typeparam>
    /// <typeparam name="TNextProperty"></typeparam>
    /// <param name="source"></param>
    /// <param name="navigationPropertyPath"></param>
    /// <returns></returns>
    public static IncludableSpecificationBuilder<TEntity, TNextProperty> ThenInclude<TEntity, TPreviousProperty, TNextProperty>(
        this IncludableSpecificationBuilder<TEntity, IEnumerable<TPreviousProperty>> source,
        Expression<Func<TPreviousProperty, TNextProperty>> navigationPropertyPath)
    {
        return new IncludableSpecificationBuilder<TEntity, TNextProperty>(
            source.IncludeChain
            , navigationPropertyPath);
    }

    public static IncludableSpecificationBuilder<TEntity, TNextProperty> ThenInclude<TEntity, TPreviousProperty, TNextProperty>(
        this IncludableSpecificationBuilder<TEntity, TPreviousProperty> source,
        Expression<Func<TPreviousProperty, TNextProperty>> navigationPropertyPath)
    {
        return new IncludableSpecificationBuilder<TEntity, TNextProperty>(
            source.IncludeChain
            , navigationPropertyPath);
    }
}
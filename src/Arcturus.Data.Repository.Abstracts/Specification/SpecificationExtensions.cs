namespace Arcturus.Repository.Specification;

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
    /// Adds a navigation property to the query path for the specified entity type.
    /// </summary>
    /// <remarks>This method is used to specify related entities to include in the query results. It is
    /// typically used in scenarios where eager loading of related data is required.</remarks>
    /// <typeparam name="TEntity">The type of the entity being queried.</typeparam>
    /// <typeparam name="TProperty">The type of the navigation property to include.</typeparam>
    /// <param name="spec">The specification to which the navigation property path is added.</param>
    /// <param name="navigationPropertyPath">An expression representing the navigation property path to include.</param>
    /// <returns>An <see cref="IncludableSpecificationBuilder{TEntity, TProperty}"/> that can be used to further configure the
    /// query.</returns>
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
    /// Specifies additional related data to be included in the query result.
    /// </summary>
    /// <remarks>This method is used to specify additional navigation properties to include in the query
    /// result, allowing for the inclusion of related data beyond the initial include operation.</remarks>
    /// <typeparam name="TEntity">The type of the entity being queried.</typeparam>
    /// <typeparam name="TPreviousProperty">The type of the previous property in the include chain.</typeparam>
    /// <typeparam name="TNextProperty">The type of the next property to include.</typeparam>
    /// <param name="source">The builder for the current include chain.</param>
    /// <param name="navigationPropertyPath">An expression representing the navigation property to include.</param>
    /// <returns>An <see cref="IncludableSpecificationBuilder{TEntity, TNextProperty}"/> for chaining further include operations.</returns>
    public static IncludableSpecificationBuilder<TEntity, TNextProperty> ThenInclude<TEntity, TPreviousProperty, TNextProperty>(
        this IncludableSpecificationBuilder<TEntity, IEnumerable<TPreviousProperty>> source,
        Expression<Func<TPreviousProperty, TNextProperty>> navigationPropertyPath)
    {
        return new IncludableSpecificationBuilder<TEntity, TNextProperty>(
            source.IncludeChain
            , navigationPropertyPath);
    }
    /// <summary>
    /// Adds a secondary related entity to be included in the query result.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity being queried.</typeparam>
    /// <typeparam name="TPreviousProperty">The type of the previous navigation property.</typeparam>
    /// <typeparam name="TNextProperty">The type of the next navigation property to include.</typeparam>
    /// <param name="source">The builder for the current include operation.</param>
    /// <param name="navigationPropertyPath">An expression representing the navigation property path for the next related entity to include.</param>
    /// <returns>An <see cref="IncludableSpecificationBuilder{TEntity, TNextProperty}"/> that can be used to further specify
    /// related entities to include.</returns>
    public static IncludableSpecificationBuilder<TEntity, TNextProperty> ThenInclude<TEntity, TPreviousProperty, TNextProperty>(
        this IncludableSpecificationBuilder<TEntity, TPreviousProperty> source,
        Expression<Func<TPreviousProperty, TNextProperty>> navigationPropertyPath)
    {
        return new IncludableSpecificationBuilder<TEntity, TNextProperty>(
            source.IncludeChain
            , navigationPropertyPath);
    }

    /// <summary>
    /// Clears all criteria from the current specification, resulting in a specification with no conditions.
    /// </summary>
    /// <typeparam name="T">The type of the entity to which the specification applies.</typeparam>
    /// <param name="specification">The specification to be cleared. Cannot be null.</param>
    /// <returns>A new specification with no conditions.</returns>
    public static Specification<T> Clear<T>(this Specification<T> specification)
        => specification.InnerClear();
    /// <summary>
    /// Clears all criteria from the current specification, resulting in a specification with no conditions.
    /// </summary>
    /// <typeparam name="T">The type of the entity to which the specification applies.</typeparam>
    /// <param name="specification">The specification to be cleared. Cannot be null.</param>
    /// <returns>A new specification with no conditions.</returns>
    public static Specification<T, R> Clear<T, R>(this Specification<T, R> specification)
        => (Specification<T, R>)specification.InnerClear();
    /// <summary>
    /// Removes any limit constraints from the specified specification.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the specification.</typeparam>
    /// <param name="specification">The specification from which to clear limit constraints.</param>
    /// <returns>A new <see cref="Specification{T}"/> instance without limit constraints.</returns>
    public static Specification<T> ClearTake<T>(this Specification<T> specification)
        => specification.InnerClearLimit();
    /// <summary>
    /// Removes any limit constraints from the specified specification.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the specification.</typeparam>
    /// <param name="specification">The specification from which to clear limit constraints.</param>
    /// <returns>A new <see cref="Specification{T}"/> instance without limit constraints.</returns>
    public static Specification<T, R> ClearTake<T, R>(this Specification<T, R> specification)
        => (Specification<T, R>)specification.InnerClearLimit();
    /// <summary>
    /// Removes any skip operation from the current specification.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the specification.</typeparam>
    /// <param name="specification">The specification from which to clear the skip operation.</param>
    /// <returns>A new specification without any skip operation applied.</returns>
    public static Specification<T> ClearSkip<T>(this Specification<T> specification)
        => specification.InnerClearSkip();
    /// <summary>
    /// Removes any skip operation from the current specification.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the specification.</typeparam>
    /// <param name="specification">The specification from which to clear the skip operation.</param>
    /// <returns>A new specification without any skip operation applied.</returns>
    public static Specification<T, R> ClearSkip<T, R>(this Specification<T, R> specification)
        => (Specification<T, R>)specification.InnerClearSkip();
    /// <summary>
    /// Removes all conditions from the current specification.
    /// </summary>
    /// <typeparam name="T">The type of the elements to which the specification applies.</typeparam>
    /// <param name="specification">The specification instance from which to clear conditions.</param>
    /// <returns>A new specification with all conditions removed.</returns>
    public static Specification<T> ClearWhere<T>(this Specification<T> specification)
        => specification.InnerClearWhere();
    /// <summary>
    /// Removes all conditions from the current specification.
    /// </summary>
    /// <typeparam name="T">The type of the elements to which the specification applies.</typeparam>
    /// <param name="specification">The specification instance from which to clear conditions.</param>
    /// <returns>A new specification with all conditions removed.</returns>
    public static Specification<T, R> ClearWhere<T, R>(this Specification<T, R> specification)
        => (Specification<T, R>)specification.InnerClearWhere();
    /// <summary>
    /// Removes all ordering criteria from the current specification.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the specification.</typeparam>
    /// <param name="specification">The specification from which to clear ordering criteria.</param>
    /// <returns>The modified specification with all ordering criteria removed.</returns>
    public static Specification<T> ClearOrderBy<T>(this Specification<T> specification)
        => specification.InnerClearOrderBy();
    /// <summary>
    /// Removes all ordering criteria from the current specification.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the specification.</typeparam>
    /// <typeparam name="R">The type of the result produced by the specification.</typeparam>
    /// <param name="specification">The specification from which to clear ordering criteria.</param>
    /// <returns>A new specification instance without any ordering criteria.</returns>
    public static Specification<T, R> ClearOrderBy<T, R>(this Specification<T, R> specification)
        => (Specification<T, R>)specification.InnerClearOrderBy();
    /// <summary>
    /// Removes all include expressions from the specified specification.
    /// </summary>
    /// <typeparam name="T">The type of the entity for which the specification is defined.</typeparam>
    /// <param name="specification">The specification from which to clear include expressions.</param>
    /// <returns>The modified specification with all include expressions removed.</returns>
    public static Specification<T> ClearIncludes<T>(this Specification<T> specification)
           => specification.InnerClearIncludes();
    /// <summary>
    /// Removes all include expressions from the specified specification.
    /// </summary>
    /// <typeparam name="T">The type of the entity for which the specification is defined.</typeparam>
    /// <typeparam name="R">The type of the result produced by the specification.</typeparam>
    /// <param name="specification">The specification from which to clear include expressions.</param>
    /// <returns>A new specification instance with all include expressions removed.</returns>
    public static Specification<T, R> ClearIncludes<T, R>(this Specification<T, R> specification)
           => (Specification<T, R>)specification.InnerClearIncludes();
}
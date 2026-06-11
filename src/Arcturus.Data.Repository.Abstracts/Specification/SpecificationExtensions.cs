namespace Arcturus.Repository.Specification;

/// <summary>
/// Provides extension methods for the <see cref="Specification{T}"/> and <see cref="Specification{T, TResult}"/> classes, allowing for fluent configuration of specifications with various criteria such as filtering, ordering, projection, and query behavior settings.
/// </summary>
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
    /// <param name="spec">The specification to which the filtering condition will be added.</param>
    /// <param name="take">The maximum number of entities to return. Must be greater than or equal to zero.</param>
    /// <returns>The updated specification with the limit applied.</returns>
    public static Specification<T, R> Take<T, R>(this Specification<T, R> spec, int take)
        => (Specification<T, R>)spec.InnerTake(take);

    /// <summary>
    /// Adds a filtering condition to the current specification based on the provided predicate.
    /// </summary>
    /// <param name="spec">The specification to which the filtering condition will be added.</param>
    /// <param name="predicate">An expression that defines the filtering condition for the entity.</param>
    /// <returns>The updated specification with the added filtering condition.</returns>
    public static Specification<T> Where<T>(this Specification<T> spec, Expression<Func<T, bool>> predicate)
        => spec.InnerWhere(predicate);
    /// <summary>
    /// Adds a filtering condition to the current specification based on the provided predicate.
    /// </summary>
    /// <param name="spec">The specification to which the filtering condition will be added.</param>
    /// <param name="predicate">An expression that defines the filtering condition for the entity.</param>
    /// <returns>The updated specification with the added filtering condition.</returns>
    public static Specification<T, R> Where<T, R>(this Specification<T, R> spec, Expression<Func<T, bool>> predicate)
        => (Specification<T, R>)spec.InnerWhere(predicate);
    /// <summary>
    /// Adds an order by condition to the current specification.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="spec">The specification to which the filtering condition will be added.</param>
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
    /// <param name="spec">The specification to which the filtering condition will be added.</param>
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
    /// <param name="spec">The specification to which the filtering condition will be added.</param>
    /// <param name="selector">An expression that defines the projection from <typeparamref name="T"/> to <typeparamref name="R"/>.</param>
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
        var builder = new IncludableSpecificationBuilder<TEntity, TProperty>(navigationPropertyPath, spec);
        spec.Add(new Specification.Expressions.IncludeExpression(builder.IncludeChain));
        return builder;
    }

    /// <summary>
    /// Adds a collection navigation property to the query path, automatically unwrapping the collection type
    /// to enable proper ThenInclude chaining on the collection's item properties.
    /// </summary>
    /// <remarks>
    /// This overload specifically handles ICollection&lt;T&gt; navigation properties. When you include a collection,
    /// the returned builder is typed with the collection's item type (TCollectionItem), not the collection type itself.
    /// This enables fluent ThenInclude calls on properties of the collection items.
    /// <para>
    /// Example: spec.Include(x => x.Orders).ThenInclude(order => order.Customer)
    /// </para>
    /// </remarks>
    /// <typeparam name="TEntity">The type of the entity being queried.</typeparam>
    /// <typeparam name="TCollectionItem">The type of items in the collection navigation property.</typeparam>
    /// <param name="spec">The specification to which the collection navigation property path is added.</param>
    /// <param name="navigationPropertyPath">An expression representing the collection navigation property path to include.</param>
    /// <returns>An <see cref="IncludableSpecificationBuilder{TEntity, TCollectionItem}"/> typed with the collection's item type,
    /// enabling ThenInclude calls on item properties.</returns>
    public static IncludableSpecificationBuilder<TEntity, TCollectionItem> Include<TEntity, TCollectionItem>(
        this Specification<TEntity> spec,
        Expression<Func<TEntity, ICollection<TCollectionItem>>> navigationPropertyPath)
        where TEntity : class
    {
        var builder = new IncludableSpecificationBuilder<TEntity, TCollectionItem>(navigationPropertyPath, spec);
        spec.Add(new Specification.Expressions.IncludeExpression(builder.IncludeChain));
        return builder;
    }

    /// <summary>
    /// Adds a root-level navigation property to include, creating a sibling include path.
    /// This method allows chaining multiple root-level includes fluently.
    /// </summary>
    /// <remarks>
    /// Use this method to add additional root-level includes after an existing include chain.
    /// This is equivalent to calling Include on the parent specification but maintains fluent chaining.
    /// <para>
    /// Example: spec.Include(x => x.Credentials).Include(x => x.AllowedScopes)
    /// </para>
    /// </remarks>
    /// <typeparam name="TEntity">The type of the entity being queried.</typeparam>
    /// <typeparam name="TPreviousProperty">The type of the previous property in the chain.</typeparam>
    /// <typeparam name="TProperty">The type of the navigation property to include.</typeparam>
    /// <param name="source">The current includable builder.</param>
    /// <param name="navigationPropertyPath">An expression representing the navigation property path to include at root level.</param>
    /// <returns>An <see cref="IncludableSpecificationBuilder{TEntity, TProperty}"/> for the new root-level include.</returns>
    public static IncludableSpecificationBuilder<TEntity, TProperty> Include<TEntity, TPreviousProperty, TProperty>(
        this IncludableSpecificationBuilder<TEntity, TPreviousProperty> source,
        Expression<Func<TEntity, TProperty>> navigationPropertyPath)
        where TEntity : class
    {
        // The previous builder's chain is already registered in the specification
        // Just create a new root-level include
        var builder = new IncludableSpecificationBuilder<TEntity, TProperty>(navigationPropertyPath, source.Specification);
        source.Specification.Add(new Specification.Expressions.IncludeExpression(builder.IncludeChain));
        return builder;
    }

    /// <summary>
    /// Adds a root-level collection navigation property to include, creating a sibling include path.
    /// Automatically unwraps the collection type to enable proper ThenInclude chaining.
    /// </summary>
    /// <remarks>
    /// This overload handles ICollection&lt;T&gt; navigation properties at the root level.
    /// <para>
    /// Example: spec.Include(x => x.Environment).Include(x => x.AllowedScopes).ThenInclude(s => s.Resource)
    /// </para>
    /// </remarks>
    /// <typeparam name="TEntity">The type of the entity being queried.</typeparam>
    /// <typeparam name="TPreviousProperty">The type of the previous property in the chain.</typeparam>
    /// <typeparam name="TCollectionItem">The type of items in the collection navigation property.</typeparam>
    /// <param name="source">The current includable builder.</param>
    /// <param name="navigationPropertyPath">An expression representing the collection navigation property path to include.</param>
    /// <returns>An <see cref="IncludableSpecificationBuilder{TEntity, TCollectionItem}"/> typed with the collection's item type.</returns>
    public static IncludableSpecificationBuilder<TEntity, TCollectionItem> Include<TEntity, TPreviousProperty, TCollectionItem>(
        this IncludableSpecificationBuilder<TEntity, TPreviousProperty> source,
        Expression<Func<TEntity, ICollection<TCollectionItem>>> navigationPropertyPath)
        where TEntity : class
    {
        // The previous builder's chain is already registered in the specification
        // Just create a new root-level include with collection unwrapping
        var builder = new IncludableSpecificationBuilder<TEntity, TCollectionItem>(navigationPropertyPath, source.Specification);
        source.Specification.Add(new Specification.Expressions.IncludeExpression(builder.IncludeChain));
        return builder;
    }


    /// <summary>
    /// Returns the underlying specification from an includable specification builder, allowing further composition or
    /// execution of the specification.
    /// </summary>
    /// <remarks>Use this method to access the parent specification when working with chained include
    /// operations, enabling additional configuration or execution.</remarks>
    /// <typeparam name="TEntity">The type of the entity being queried.</typeparam>
    /// <typeparam name="TNextProperty">The type of the next property in the include chain.</typeparam>
    /// <param name="source">The includable specification builder from which to retrieve the parent specification. Cannot be null.</param>
    /// <returns>The specification associated with the provided includable specification builder.</returns>
    public static Specification<TEntity> Parent<TEntity, TNextProperty>(
        this IncludableSpecificationBuilder<TEntity, TNextProperty> source)
    {
        return source.Specification;
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
            , source.IncludeChain  // Parent chain is the current chain for ThenInclude
            , navigationPropertyPath
            , source.Specification);
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
            , source.IncludeChain  // Parent chain is the current chain for ThenInclude
            , navigationPropertyPath
            , source.Specification);
    }

    /// <summary>
    /// Adds a sibling navigation property at the same nesting level as the current include.
    /// </summary>
    /// <remarks>
    /// This method creates a new include path that starts from the same parent level as the current builder.
    /// It allows you to include multiple properties at the same nesting depth without navigating back up the hierarchy.
    /// <para>
    /// Example: spec.Include(x => x.AllowedScopes).ThenInclude(s => s.Resource).AndInclude(s => s.Owner)
    /// This includes both Resource and Owner as siblings, both nested under AllowedScopes.
    /// </para>
    /// </remarks>
    /// <typeparam name="TEntity">The type of the entity being queried.</typeparam>
    /// <typeparam name="TPreviousProperty">The type of the previous property in the chain.</typeparam>
    /// <typeparam name="TNextProperty">The type of the sibling property to include.</typeparam>
    /// <param name="source">The builder for the current include chain.</param>
    /// <param name="navigationPropertyPath">An expression representing the sibling navigation property to include.</param>
    /// <returns>An <see cref="IncludableSpecificationBuilder{TEntity, TNextProperty}"/> for the new sibling include path.</returns>
    public static IncludableSpecificationBuilder<TEntity, TNextProperty> AndInclude<TEntity, TPreviousProperty, TNextProperty>(
        this IncludableSpecificationBuilder<TEntity, TPreviousProperty> source,
        Expression<Func<TPreviousProperty, TNextProperty>> navigationPropertyPath)
    {
        // The current builder's chain is already registered
        // Create a new sibling chain starting from the parent level
        var siblingChain = new List<LambdaExpression>(source.ParentChain);
        var builder = new IncludableSpecificationBuilder<TEntity, TNextProperty>(
            siblingChain
            , source.ParentChain  // Parent chain remains the same for siblings
            , navigationPropertyPath
            , source.Specification);

        // Register this new sibling chain
        source.Specification.Add(new Specification.Expressions.IncludeExpression(builder.IncludeChain));
        return builder;
    }

    /// <summary>
    /// Adds a sibling collection navigation property at the same nesting level as the current include.
    /// Automatically unwraps the collection type to enable proper ThenInclude chaining.
    /// </summary>
    /// <remarks>
    /// This overload handles ICollection&lt;T&gt; navigation properties as siblings at the same nesting level.
    /// <para>
    /// Example: spec.Include(x => x.User).ThenInclude(u => u.Profile).AndInclude(u => u.Orders).ThenInclude(o => o.Items)
    /// This includes Profile and Orders as siblings under User, with Orders unwrapped for further navigation.
    /// </para>
    /// </remarks>
    /// <typeparam name="TEntity">The type of the entity being queried.</typeparam>
    /// <typeparam name="TPreviousProperty">The type of the previous property in the chain.</typeparam>
    /// <typeparam name="TCollectionItem">The type of items in the sibling collection navigation property.</typeparam>
    /// <param name="source">The builder for the current include chain.</param>
    /// <param name="navigationPropertyPath">An expression representing the sibling collection navigation property to include.</param>
    /// <returns>An <see cref="IncludableSpecificationBuilder{TEntity, TCollectionItem}"/> typed with the collection's item type.</returns>
    public static IncludableSpecificationBuilder<TEntity, TCollectionItem> AndInclude<TEntity, TPreviousProperty, TCollectionItem>(
        this IncludableSpecificationBuilder<TEntity, TPreviousProperty> source,
        Expression<Func<TPreviousProperty, ICollection<TCollectionItem>>> navigationPropertyPath)
    {
        // The current builder's chain is already registered
        // Create a new sibling chain starting from the parent level with collection unwrapping
        var siblingChain = new List<LambdaExpression>(source.ParentChain);
        var builder = new IncludableSpecificationBuilder<TEntity, TCollectionItem>(
            siblingChain
            , source.ParentChain  // Parent chain remains the same for siblings
            , navigationPropertyPath
            , source.Specification);

        // Register this new sibling chain
        source.Specification.Add(new Specification.Expressions.IncludeExpression(builder.IncludeChain));
        return builder;
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
    /// <typeparam name="R">The type of the result produced by the specification.</typeparam>
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
    /// <typeparam name="R">The type of the result produced by the specification.</typeparam>
    /// <param name="specification">The specification from which to clear limit constraints.</param>
    /// <returns>A new <see cref="Specification{T, R}"/> instance without limit constraints.</returns>
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
    /// <typeparam name="R">The type of the result produced by the specification.</typeparam>
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
    /// <typeparam name="R">The type of the result produced by the specification.</typeparam>
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
namespace Arcturus.Repository.Specification;

/// <summary>
/// Provides extension methods for including related entities in a specification. These methods allow you to specify
/// </summary>
public static class SpecificationIncludeExtensions
{
    /// <summary>
    /// Includes a related entity in the specification. This method is used to specify a navigation property that should be included
    /// in the query results.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity being queried.</typeparam>
    /// <typeparam name="TProperty">The type of the related property being included.</typeparam>
    /// <param name="specification">The specification to which the include should be added.</param>
    /// <param name="navigation">A lambda expression representing the navigation property to include.</param>
    /// <returns></returns>
    public static IIncludeBuilder<TEntity, TProperty, TProperty>Include<TEntity, TProperty>(
        this Specification<TEntity> specification,
        Expression<Func<TEntity, TProperty?>> navigation) where TProperty : class
    {
        var name = GetMemberName(navigation);
        specification.Add(new Expressions.IncludeExpression(name));

        return new IncludeBuilder<TEntity, TProperty, TProperty>(
            specification,
            rootPath: name,
            currentPath: name);
    }
    /// <summary>
    /// Includes a collection of related entities in the specification. This method is used to specify a navigation property that represents a collection of related entities
    /// in the query results.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity being queried.</typeparam>
    /// <typeparam name="TElement">The type of the elements in the collection being included.</typeparam>
    /// <param name="specification">The specification to which the include should be added.</param>
    /// <param name="navigation">A lambda expression representing the navigation property to include.</param>
    /// <returns>An include builder for chaining additional includes.</returns>
    public static IIncludeBuilder<TEntity, TElement, TElement>Include<TEntity, TElement>(
        this Specification<TEntity> specification,
        Expression<Func<TEntity, ICollection<TElement>>> navigation) where TElement : class
    {
        var name = GetMemberName(navigation);
        specification.Add(new Expressions.IncludeExpression(name));

        return new IncludeBuilder<TEntity, TElement, TElement>(
            specification,
            rootPath: name,
            currentPath: name);
    }
    /// <summary>
    /// Includes a related entity in the specification. This method is used to specify a navigation property that should be included
    /// in the query results.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity being queried.</typeparam>
    /// <typeparam name="TBranchRoot">The type of the root entity in the include chain.</typeparam>
    /// <typeparam name="TCurrent">The type of the current entity in the include chain.</typeparam>
    /// <typeparam name="TNext">The type of the next entity to include in the chain.</typeparam>
    /// <param name="builder">The include builder used to construct the include chain.</param>
    /// <param name="navigation">A lambda expression representing the navigation property to include.</param>
    /// <returns>An include builder for chaining additional includes.</returns>
    public static IIncludeBuilder<TEntity, TNext, TNext>
        Include<TEntity, TBranchRoot, TCurrent, TNext>(
            this IIncludeBuilder<TEntity, TBranchRoot, TCurrent> builder,
            Expression<Func<TEntity, TNext?>> navigation)
        where TNext : class
    {
        return builder.Specification.Include(navigation);
    }
    /// <summary>
    /// Includes a collection of related entities in the specification. This method is used to specify a navigation property that represents a collection of related entities
    /// in the query results.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity being queried.</typeparam>
    /// <typeparam name="TBranchRoot">The type of the root entity in the include chain.</typeparam>
    /// <typeparam name="TCurrent">The type of the current entity in the include chain.</typeparam>
    /// <typeparam name="TElement">The type of the elements in the collection being included.</typeparam>
    /// <param name="builder">The include builder used to construct the include chain.</param>
    /// <param name="navigation">A lambda expression representing the navigation property to include.</param>
    /// <returns>An include builder for chaining additional includes.</returns>

    public static IIncludeBuilder<TEntity, TElement, TElement>
        Include<TEntity, TBranchRoot, TCurrent, TElement>(
            this IIncludeBuilder<TEntity, TBranchRoot, TCurrent> builder,
            Expression<Func<TEntity, ICollection<TElement>>> navigation)
        where TElement : class
    {
        return builder.Specification.Include(navigation);
    }
    /// <summary>
    /// Includes a related entity in the specification. This method is used to specify a navigation property that should be included
    /// in the query results.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity being queried.</typeparam>
    /// <typeparam name="TBranchRoot">The type of the root entity in the include chain.</typeparam>
    /// <typeparam name="TCurrent">The type of the current entity in the include chain.</typeparam>
    /// <typeparam name="TNext">The type of the next entity to include in the chain.</typeparam>
    /// <param name="builder">The include builder used to construct the include chain.</param>
    /// <param name="navigation">A lambda expression representing the navigation property to include.</param>
    /// <returns>An include builder for chaining additional includes.</returns>

    public static IIncludeBuilder<TEntity, TBranchRoot, TNext>
        ThenInclude<TEntity, TBranchRoot, TCurrent, TNext>(
            this IIncludeBuilder<TEntity, TBranchRoot, TCurrent> builder,
            Expression<Func<TCurrent, TNext?>> navigation)
        where TNext : class
    {
        var name = GetMemberName(navigation);
        var path = $"{builder.CurrentPath}.{name}";

        builder.Specification.Add(new Expressions.IncludeExpression(path));

        return new IncludeBuilder<TEntity, TBranchRoot, TNext>(
            builder.Specification,
            builder.RootPath,
            path);
    }
    /// <summary>
    /// Includes a collection of related entities in the specification. This method is used to specify a navigation property that represents a collection of related entities
    /// in the query results.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity being queried.</typeparam>
    /// <typeparam name="TBranchRoot">The type of the root entity in the include chain.</typeparam>
    /// <typeparam name="TCurrent">The type of the current entity in the include chain.</typeparam>
    /// <typeparam name="TElement">The type of the elements in the collection being included.</typeparam>
    /// <param name="builder">The include builder used to construct the include chain.</param>
    /// <param name="navigation">A lambda expression representing the navigation property to include.</param>
    /// <returns>An include builder for chaining additional includes.</returns>
    public static IIncludeBuilder<TEntity, TBranchRoot, TElement>
        ThenInclude<TEntity, TBranchRoot, TCurrent, TElement>(
            this IIncludeBuilder<TEntity, TBranchRoot, TCurrent> builder,
            Expression<Func<TCurrent, ICollection<TElement>>> navigation)
        where TElement : class
    {
        var name = GetMemberName(navigation);
        var path = $"{builder.CurrentPath}.{name}";

        builder.Specification.Add(new Expressions.IncludeExpression(path));

        return new IncludeBuilder<TEntity, TBranchRoot, TElement>(
            builder.Specification,
            builder.RootPath,
            path);
    }
    /// <summary>
    /// Includes a related entity in the specification. This method is used to specify a navigation property that should be included
    /// in the query results.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity being queried.</typeparam>
    /// <typeparam name="TBranchRoot">The type of the root entity in the include chain.</typeparam>
    /// <typeparam name="TCurrent">The type of the current entity in the include chain.</typeparam>
    /// <typeparam name="TNext">The type of the next entity to include in the chain.</typeparam>
    /// <param name="builder">The include builder used to construct the include chain.</param>
    /// <param name="navigation">A lambda expression representing the navigation property to include.</param>
    /// <returns>An include builder for chaining additional includes.</returns>
    public static IIncludeBuilder<TEntity, TBranchRoot, TNext>
        AndInclude<TEntity, TBranchRoot, TCurrent, TNext>(
            this IIncludeBuilder<TEntity, TBranchRoot, TCurrent> builder,
            Expression<Func<TBranchRoot, TNext?>> navigation)
        where TNext : class
    {
        var name = GetMemberName(navigation);
        var path = $"{builder.RootPath}.{name}";

        builder.Specification.Add(new Expressions.IncludeExpression(path));

        return new IncludeBuilder<TEntity, TBranchRoot, TNext>(
            builder.Specification,
            builder.RootPath,
            path);
    }
    /// <summary>
    /// Includes a collection of related entities in the specification. This method is used to specify a navigation property that represents a collection of related entities
    /// in the query results.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity being queried.</typeparam>
    /// <typeparam name="TBranchRoot">The type of the root entity in the include chain.</typeparam>
    /// <typeparam name="TCurrent">The type of the current entity in the include chain.</typeparam>
    /// <typeparam name="TElement">The type of the elements in the collection being included.</typeparam>
    /// <param name="builder">The include builder used to construct the include chain.</param>
    /// <param name="navigation">A lambda expression representing the navigation property to include.</param>
    /// <returns>An include builder for chaining additional includes.</returns>
    public static IIncludeBuilder<TEntity, TBranchRoot, TElement>
        AndInclude<TEntity, TBranchRoot, TCurrent, TElement>(
            this IIncludeBuilder<TEntity, TBranchRoot, TCurrent> builder,
            Expression<Func<TBranchRoot, ICollection<TElement>>> navigation)
        where TElement : class
    {
        var name = GetMemberName(navigation);
        var path = $"{builder.RootPath}.{name}";

        builder.Specification.Add(new Expressions.IncludeExpression(path));

        return new IncludeBuilder<TEntity, TBranchRoot, TElement>(
            builder.Specification,
            builder.RootPath,
            path);
    }

    private static string GetMemberName(LambdaExpression expression)
    {
        Expression body = expression.Body;

        if (body is UnaryExpression unaryExpression)
            body = unaryExpression.Operand;

        if (body is not MemberExpression memberExpression)
            throw new InvalidOperationException(
                $"Expression '{expression}' is not a valid member access.");

        return memberExpression.Member.Name;
    }
}

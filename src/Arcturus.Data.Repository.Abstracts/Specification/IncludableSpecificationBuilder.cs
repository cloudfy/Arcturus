namespace Arcturus.Repository.Specification;

/// <summary>
/// Provides a builder for constructing a chain of include expressions to specify related entities to be included in a
/// query for the specified entity type.
/// </summary>
/// <remarks>This class is used to build a chain of include expressions, allowing the caller to specify multiple
/// levels of related entities to include in a query. The include chain is represented as a list of lambda
/// expressions.</remarks>
/// <typeparam name="TEntity">The type of the entity being queried.</typeparam>
/// <typeparam name="TProperty">The type of the related property being included.</typeparam>
public sealed class IncludableSpecificationBuilder<TEntity, TProperty>
{
    /// <summary>
    /// Gets the collection of lambda expressions representing the include chain for query navigation.
    /// </summary>
    public List<LambdaExpression> IncludeChain { get; }

    internal Specification<TEntity> Specification { get; private set; }

    /// <summary>
    /// Gets the parent chain of expressions representing the navigation path up to the parent level.
    /// Used for AndInclude operations to create sibling includes at the same nesting level.
    /// </summary>
    internal List<LambdaExpression> ParentChain { get; }

    internal IncludableSpecificationBuilder(Expression<Func<TEntity, TProperty>> root, Specification<TEntity> specification)
    {
        IncludeChain = [root];
        ParentChain = [];
        Specification = specification;
    }
    /// <summary>
    /// Initializes a new instance with an untyped lambda expression root.
    /// Used for type unwrapping scenarios (e.g., ICollection&lt;T&gt; to T).
    /// </summary>
    internal IncludableSpecificationBuilder(LambdaExpression root, Specification<TEntity> specification)
    {
        IncludeChain = [root];
        ParentChain = [];
        Specification = specification;
    }
    internal IncludableSpecificationBuilder(List<LambdaExpression> chain, LambdaExpression next, Specification<TEntity> specification)
    {
        IncludeChain = [.. chain, next];
        // Parent chain is everything except the last expression
        ParentChain = chain.Count > 0 ? [.. chain] : [];
        Specification = specification;
    }

    /// <summary>
    /// Initializes a new instance with explicit parent chain tracking for AndInclude scenarios.
    /// </summary>
    internal IncludableSpecificationBuilder(
        List<LambdaExpression> chain, 
        List<LambdaExpression> parentChain,
        LambdaExpression next, 
        Specification<TEntity> specification)
    {
        IncludeChain = [.. chain, next];
        ParentChain = [.. parentChain];
        Specification = specification;
    }
}
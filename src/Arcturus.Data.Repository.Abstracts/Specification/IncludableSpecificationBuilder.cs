using System.Linq.Expressions;

namespace Arcturus.Data.Repository.Abstracts;

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

    internal IncludableSpecificationBuilder(Expression<Func<TEntity, TProperty>> root)
    {
        IncludeChain = [root];
    }

    internal IncludableSpecificationBuilder(List<LambdaExpression> chain, LambdaExpression next)
    {
        IncludeChain = [.. chain, next];
    }
}
using System.Linq.Expressions;

namespace Arcturus.Data.Repository.Abstracts;

public sealed class IncludableSpecificationBuilder<TEntity, TProperty>
{
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

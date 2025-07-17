using System.Linq.Expressions;

namespace Arcturus.Data.Repository.Abstracts.Specification.Expressions;

public sealed class IncludeExpression
{
    private readonly List<LambdaExpression> _includeChain;

    internal IncludeExpression(List<LambdaExpression> includeChain)
    {
        _includeChain = includeChain;
    }

    public IEnumerable<LambdaExpression> Chains => _includeChain;
}
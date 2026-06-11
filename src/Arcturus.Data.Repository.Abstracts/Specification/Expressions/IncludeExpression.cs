namespace Arcturus.Repository.Specification.Expressions;

/// <summary>
/// Represents a collection of lambda expressions that define a chain of include operations.
/// </summary>
/// <remarks>This class is used to encapsulate a sequence of lambda expressions, typically used in scenarios where
/// multiple related entities are included in a query. The expressions in the chain are evaluated in order to determine
/// the navigation paths to include.</remarks>
public sealed class IncludeExpression
{
    private readonly string _includeChain;

    internal IncludeExpression(string includeChain)
    {
        _includeChain = includeChain;
    }
    /// <summary>
    /// Gets the collection of lambda expressions representing the include chains.
    /// </summary>
    public IEnumerable<string> Chains => [ _includeChain ];
}
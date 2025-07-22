namespace Arcturus.Repository.Specification.Expressions;

/// <summary>
/// Represents a filter expression used to determine which elements satisfy a specified condition.
/// </summary>
/// <remarks>This class encapsulates a filter expression and provides a compiled function for evaluating the
/// condition.</remarks>
/// <typeparam name="T">The type of elements to which the filter expression is applied.</typeparam>
public sealed class WhereExpression<T>
{
    private Func<T, bool>? _filterFunc;
    internal WhereExpression(Expression<Func<T, bool>> filter)
    {
        _ = filter ?? throw new ArgumentNullException(nameof(filter));
        Filter = filter;
    }
    /// <summary>
    /// Gets the filter expression used to determine which elements satisfy a specified condition.
    /// </summary>
    public Expression<Func<T, bool>> Filter { get; }
    internal Func<T, bool> FilterFunc => _filterFunc ??= Filter.Compile();
}

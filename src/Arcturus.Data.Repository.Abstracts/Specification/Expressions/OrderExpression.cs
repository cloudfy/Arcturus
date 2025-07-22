namespace Arcturus.Repository.Specification.Expressions;

/// <summary>
/// Represents an expression used to define the ordering of elements in a queryable collection.
/// </summary>
/// <remarks>This class encapsulates an expression that specifies the property or field by which to order the
/// results, along with a flag indicating whether the order should be descending.</remarks>
/// <typeparam name="T">The type of elements in the collection to be ordered.</typeparam>
public sealed class OrderExpression<T>
{
    private Func<T, object?>? _orderFunc;
    internal OrderExpression(Expression<Func<T, object?>> orderBy, bool descending)
    {
        OrderBy = orderBy;
        Descending = descending;
    }
    /// <summary>
    /// Gets the expression used to specify the property or field by which to order the results.
    /// </summary>
    public Expression<Func<T, object?>> OrderBy { get; }
    /// <summary>
    /// Gets a value indicating whether the sorting order is descending.
    /// </summary>
    public bool Descending { get; }
    internal Func<T, object?> OrderByFunc => _orderFunc ??= OrderBy.Compile();

}

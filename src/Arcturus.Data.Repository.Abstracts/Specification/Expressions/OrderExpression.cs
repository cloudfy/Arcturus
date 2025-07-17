using System.Linq.Expressions;

namespace Arcturus.Data.Repository.Abstracts.Specification.Expressions;

public sealed class OrderExpression<T>
{
    private Func<T, object?>? _orderFunc;
    internal OrderExpression(Expression<Func<T, object?>> orderBy, bool descending)
    {
        OrderBy = orderBy;
        Descending = descending;
    }
    public Expression<Func<T, object?>> OrderBy { get; }
    public bool Descending { get; }
    public Func<T, object?> OrderByFunc => _orderFunc ??= OrderBy.Compile();

}

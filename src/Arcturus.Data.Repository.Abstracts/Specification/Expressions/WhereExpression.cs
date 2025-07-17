using System.Linq.Expressions;

namespace Arcturus.Data.Repository.Abstracts.Specification.Expressions;

public sealed class WhereExpression<T>
{
    private Func<T, bool>? _filterFunc;
    internal WhereExpression(Expression<Func<T, bool>> filter)
    {
        _ = filter ?? throw new ArgumentNullException(nameof(filter));
        Filter = filter;
    }
    public Expression<Func<T, bool>> Filter { get; }
    public Func<T, bool> FilterFunc => _filterFunc ??= Filter.Compile();
}

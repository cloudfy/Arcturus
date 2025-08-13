using Arcturus.Repository.Specification.Expressions;

namespace Arcturus.Repository.Specification;

/// <summary>
/// Represents a specification for querying <typeparamref name="T"/>, including filtering, sorting, and related data inclusion.
/// </summary>
/// <remarks>This class encapsulates query criteria, such as filtering conditions, sorting rules, and inclusion of
/// related data. It is commonly used in repository patterns or query-building scenarios to define reusable query
/// logic.</remarks>
/// <typeparam name="T">The type of entity to which the specification applies.</typeparam>
public class Specification<T>()
    : ISpecification<T>
{
    private readonly List<WhereExpression<T>> _whereExpressions = [];
    private readonly List<OrderExpression<T>> _orderExpressions = [];
    private readonly List<IncludeExpression> _includeExpressions = [];

    public IEnumerable<WhereExpression<T>> WhereExpressions => _whereExpressions;
    public IEnumerable<OrderExpression<T>> OrderExpressions => _orderExpressions;
    public IEnumerable<IncludeExpression> IncludeExpressions => _includeExpressions;

    internal void Add(WhereExpression<T> whereExpression) => _whereExpressions.Add(whereExpression);
    internal void Add(OrderExpression<T> orderExpression) => _orderExpressions.Add(orderExpression);
    internal void Add(IncludeExpression includeExpression) => _includeExpressions.Add(includeExpression);

    internal Specification<T> InnerOrderBy(
        Expression<Func<T, object?>> orderByExpression, bool descending = false)
    {
        Add(new OrderExpression<T>(orderByExpression, descending));
        return this;
    }
    internal Specification<T> InnerTake(int take)
    {
        Limit = take;
        return this;
    }
    internal Specification<T> InnerWhere(Expression<Func<T, bool>> predicate)
    {
        Add(new WhereExpression<T>(predicate));
        return this;
    }
    internal Specification<T> InnerWithSplitQuery(bool value = true)
    {
        UseSplitQuery = value;
        return this;
    }
    internal Specification<T> InnerWithIgnoreQueryFilters(bool value = true)
    {
        IgnoreQueryFilters = value;
        return this;
    }
    internal Specification<T, TResult> Clone<TResult>()
    {
        var newSpec = new Specification<T, TResult>();
        CopyState(this, newSpec);
        return newSpec;
    }
    private static void CopyState(Specification<T> source, Specification<T> target)
    {
        target.UseSplitQuery = source.UseSplitQuery;
        target.IgnoreQueryFilters = source.IgnoreQueryFilters;
        target.Limit = source.Limit;
        target.Skip = source.Skip;
        foreach (var item in source.IncludeExpressions)
        {
            target.Add(item);
        }
        foreach (var item in source.WhereExpressions)
        {
            target.Add(item);
        }
        foreach (var item in source.OrderExpressions)
        {
            target.Add(item);
        }
    }

    public bool UseSplitQuery { get; internal set; }
    public bool IgnoreQueryFilters { get; internal set; }
    public int? Limit { get; internal set; }
    public int? Skip { get; internal set; }

    internal Specification<T> InnerClear()
    {
        _whereExpressions.Clear();
        _orderExpressions.Clear();
        _includeExpressions.Clear();
        UseSplitQuery = false;
        IgnoreQueryFilters = false;
        Limit = null;
        Skip = null;
        return this;
    }
    internal Specification<T> InnerClearLimit()
    {
        Limit = null;
        return this;
    }
    internal Specification<T> InnerClearSkip()
    {
        Skip = null;
        return this;
    }
    internal Specification<T> InnerClearWhere()
    {
        _whereExpressions.Clear();
        return this;
    }
    internal Specification<T> InnerClearOrderBy()
    {
        _orderExpressions.Clear();
        return this;
    }
    internal Specification<T> InnerClearIncludes()
    {
        _includeExpressions.Clear();
        return this;
    }
}

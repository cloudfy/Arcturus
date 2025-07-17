using Arcturus.Data.Repository.Abstracts.Specification.Expressions;

namespace Arcturus.Data.Repository.Abstracts;

public interface ISpecification<T>
{
    IEnumerable<WhereExpression<T>> WhereExpressions { get; }
    IEnumerable<OrderExpression<T>> OrderExpressions { get; }
    IEnumerable<IncludeExpression> IncludeExpressions { get; }

    /// <summary>
    /// Gets the maximum number of items to retrieve. 
    /// </summary>
    int? Limit { get; }
    /// <summary>
    /// Gets the number of items to skip.
    /// </summary>
    int? Skip { get; }
    /// <summary>
    /// Gets a value indicating whether split queries are used for database operations.
    /// </summary>
    bool UseSplitQuery { get; }
    /// <summary>
    /// Gets a value indicating if query filters is ignored.
    /// </summary>
    bool IgnoreQueryFilters { get; }

}
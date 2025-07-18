using Arcturus.Data.Repository.Abstracts.Specification.Expressions;

namespace Arcturus.Data.Repository.Abstracts;

/// <summary>
/// Defines a specification pattern interface for querying data with filtering, sorting, and inclusion criteria.
/// </summary>
/// <remarks>This interface provides a way to encapsulate query logic, allowing for the composition of complex
/// queries by specifying filtering, sorting, and inclusion rules. It is typically used in repository patterns to
/// abstract query details from the consumer.</remarks>
/// <typeparam name="T">The type of entity to which the specification applies.</typeparam>
public interface ISpecification<T>
{
    /// <summary>
    /// Gets the collection of where expressions used to filter query results.
    /// </summary>
    IEnumerable<WhereExpression<T>> WhereExpressions { get; }
    /// <summary>
    /// Gets the collection of order expressions used to define the sorting criteria.
    /// </summary>
    IEnumerable<OrderExpression<T>> OrderExpressions { get; }
    /// <summary>
    /// Gets the collection of include expressions used to specify related entities to include in query results.
    /// </summary>
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
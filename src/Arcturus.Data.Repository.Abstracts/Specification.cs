using System.Linq.Expressions;

namespace Arcturus.Data.Repository.Abstracts;

/// <summary>
/// Represents a specification for querying <typeparamref name="T"/>, including filtering, sorting, and related data inclusion.
/// </summary>
/// <remarks>This class encapsulates query criteria, such as filtering conditions, sorting rules, and inclusion of
/// related data. It is commonly used in repository patterns or query-building scenarios to define reusable query
/// logic.</remarks>
/// <typeparam name="T">The type of entity to which the specification applies.</typeparam>
/// <param name="predicate">Optional. Where predicate to apply.</param>
public sealed class Specification<T>(Expression<Func<T, bool>>? predicate)
{
    private readonly Expression<Func<T, bool>>? _predicate = predicate;
    private readonly List<List<LambdaExpression>> _includeChains = [];
    private readonly List<(Expression, bool)> _orderBy = [];
    private readonly List<Expression<Func<T, bool>>> _where = [predicate ?? (x => true)];

    internal void AddIncludeChain(List<LambdaExpression> chain) => _includeChains.Add(chain);

    internal void AddOrderBy<TEntity, TProperty>(
        Expression<Func<TEntity, TProperty>> orderByExpression, bool descending = false)
    {
        _orderBy.Add((orderByExpression, descending));
    }
    internal void AddWhere(Expression<Func<T, bool>> predicate)
    {
        _where.Add(predicate);
    }

    public IEnumerable<List<LambdaExpression>> IncludeChains => _includeChains;
    public IEnumerable<(Expression, bool)> OrderBy => _orderBy;
    public IEnumerable<Expression<Func<T, bool>>> Where => _where;
    /// <summary>
    /// Gets a value indicating whether split queries are used for database operations.
    /// </summary>
    public bool UseSplitQuery { get; internal set; }
    /// <summary>
    /// Gets the maximum number of items to process or retrieve. 
    /// </summary>
    public int? Limit { get; internal set; }
    /// <summary>
    /// Gets the predicate expression used to filter items of type <typeparamref name="T"/>.
    /// </summary>
    public Expression<Func<T, bool>>? Predicate => _predicate;
}

using System.Linq.Expressions;

namespace Arcturus.Data.Repository.Abstracts;

public sealed class Specification<T>(Expression<Func<T, bool>> predicate)
{
    private readonly Expression<Func<T, bool>> _predicate = predicate;
    private readonly List<List<LambdaExpression>> _includeChains = [];
    private readonly List<(Expression, bool)> _orderBy = [];

    internal void AddIncludeChain(List<LambdaExpression> chain) => _includeChains.Add(chain);

    internal void AddOrderBy<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> orderByExpression, bool descending = false)
    {
        _orderBy.Add((orderByExpression, descending));
    }

    public IEnumerable<List<LambdaExpression>> IncludeChains => _includeChains;
    public IEnumerable<(Expression, bool)> OrderBy => _orderBy;
    public bool UseSplitQuery { get; internal set; }

    public Expression<Func<T, bool>> Predicate = predicate;
}

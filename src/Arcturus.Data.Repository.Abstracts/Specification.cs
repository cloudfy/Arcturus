using System.Linq.Expressions;

namespace Arcturus.Data.Repository.Abstracts;

/// <summary>
/// Represents a specification that defines criteria for querying objects of type <typeparamref name="T"/>.
/// </summary>
/// <remarks>A specification encapsulates a predicate expression that determines whether an object satisfies the
/// criteria. It also supports including related properties for eager loading in query scenarios.</remarks>
/// <typeparam name="T">The type of object to which the specification applies.</typeparam>
/// <param name="predicate"></param>
public sealed class Specification<T>(Expression<Func<T, bool>> predicate)
{
    private readonly Expression<Func<T, bool>> _predicate = predicate;
    private readonly List<Expression<Func<T, object>>> _includes = [];

    public Expression<Func<T, bool>> Predicate => _predicate;
    public IEnumerable<Expression<Func<T, object>>> Includes => _includes;

    internal void AddInclude(Expression<Func<T, object>> includeExpression)
    {
        _includes.Add(includeExpression);
    }
}
using System.Linq.Expressions;

namespace Arcturus.Data.Repository.Abstracts;

public class Specification<T, TResult> : Specification<T>, ISpecification<T, TResult>
{
    public Expression<Func<T, TResult>>? Projection { get; private set; }

    internal Specification<T, TResult> InnerProject(Expression<Func<T, TResult>> selector)
    {
        Projection = selector;
        return this;
    }
}

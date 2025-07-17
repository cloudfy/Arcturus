using System.Linq.Expressions;

namespace Arcturus.Data.Repository.Abstracts;

public interface ISpecification<TEntity, TResult> : ISpecification<TEntity>
{
    Expression<Func<TEntity, TResult>>? Projection { get; }
}
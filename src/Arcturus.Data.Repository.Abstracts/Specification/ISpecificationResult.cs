namespace Arcturus.Repository.Specification;

/// <summary>
/// Defines a specification with a projection for querying entities of type <typeparamref name="TEntity"/>.
/// </summary>
/// <remarks>This interface extends <see cref="ISpecification{TEntity}"/> by adding a projection capability,
/// allowing the specification to transform the entity into a different result type.</remarks>
/// <typeparam name="TEntity">The type of the entity to which the specification applies.</typeparam>
/// <typeparam name="TResult">The type of the result produced by the projection.</typeparam>
public interface ISpecification<TEntity, TResult> : ISpecification<TEntity>
{
    /// <summary>
    /// Gets the expression used to project an entity of type <typeparamref name="TEntity"/> to a result of type
    /// <typeparamref name="TResult"/>.
    /// </summary>
    Expression<Func<TEntity, TResult>>? Projection { get; }
}
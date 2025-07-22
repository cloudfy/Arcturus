namespace Arcturus.Repository.Specification;

/// <summary>
/// Represents a specification pattern that includes a projection for transforming elements of type <typeparamref
/// name="T"/> into results of type <typeparamref name="TResult"/>.
/// </summary>
/// <remarks>This class extends the base <see cref="Specification{T}"/> to include a projection expression,
/// allowing for the transformation of elements that satisfy the specification.</remarks>
/// <typeparam name="T">The type of elements to which the specification is applied.</typeparam>
/// <typeparam name="TResult">The type of the result produced by the projection.</typeparam>
public class Specification<T, TResult> : Specification<T>, ISpecification<T, TResult>
{
    /// <summary>
    /// Gets the expression used to project an object of type <typeparamref name="T"/> into an object of type
    /// <typeparamref name="TResult"/>.
    /// </summary>
    public Expression<Func<T, TResult>>? Projection { get; private set; }

    internal Specification<T, TResult> InnerProject(Expression<Func<T, TResult>> selector)
    {
        Projection = selector;
        return this;
    }

    /// <summary>
    /// Clears the current projection expression, effectively removing any transformation logic
    /// </summary>
    /// <returns></returns>
    public Specification<T, TResult> ClearProjection()
    {
        Projection = null;
        return this;
    }
}

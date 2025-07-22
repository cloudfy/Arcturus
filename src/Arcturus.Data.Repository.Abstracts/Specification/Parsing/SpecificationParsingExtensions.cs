using Arcturus.Repository.Specification.Parsing.Internals;

namespace Arcturus.Repository.Specification.Parsing;

/// <summary>
/// Provides extension methods for creating and manipulating specifications with filtering and ordering capabilities.
/// </summary>
/// <remarks>This static class includes methods to apply filters and order elements within specifications.  It
/// supports both single-type and dual-type specifications, allowing for flexible querying and sorting.</remarks>
public static class SpecificationParsingExtensions
{
    /// <summary>
    /// Creates a new specification by applying a filter to the existing specification.
    /// <para>
    /// Example usage: id gt 'dce743ba-85d3-4275-935c-2053c468b04e'
    /// </para>
    /// </summary>
    /// <typeparam name="T">The type of the elements to which the specification applies.</typeparam>
    /// <param name="specification">The existing specification to which the filter will be applied. Cannot be null.</param>
    /// <param name="filter">The filter expression to apply. This should be a valid filter string that defines the criteria for the
    /// specification.</param>
    /// <remarks>Use operators: eq (equal), ne (not equal), lk (like), ge (greater or equal), gt (greater), lt (less), le (less or equal)</remarks>
    /// <returns>A new <see cref="Specification{T}"/> that represents the original specification with the additional filter
    /// applied.</returns>
    /// <exception cref="FilterExpressionException" />
    public static Specification<T> Where<T>(this Specification<T> specification, string filter)
        => InnerWhere(specification, filter);
    /// <summary>
    /// Filters the current specification based on the provided filter string.
    /// <para>
    /// Example usage: id gt 'dce743ba-85d3-4275-935c-2053c468b04e'
    /// </para>
    /// </summary>
    /// <typeparam name="T">Data model type.</typeparam>
    /// <typeparam name="R">Projected type.</typeparam>
    /// <param name="specification">The specification to be filtered.</param>
    /// <param name="filter">A string representing the filter criteria to apply.</param>
    /// <remarks>Use operators: eq (equal), ne (not equal), lk (like), ge (greater or equal), gt (greater), lt (less), le (less or equal)</remarks>
    /// <returns>A new <see cref="Specification{T, R}"/> that represents the filtered specification.</returns>
    /// <exception cref="FilterExpressionException" />
    public static Specification<T, R> Where<T, R>(this Specification<T, R> specification, string filter)
        => (Specification<T, R>)InnerWhere(specification, filter);

    private static Specification<T> InnerWhere<T>(this Specification<T> specification, string filter)
    {
        if (string.IsNullOrWhiteSpace(filter))
            throw new ArgumentException("Filter cannot be null or empty.", nameof(filter));

        var parsed = FilterExpressionParser.Parse<T>([filter]);
        return SpecificationExtensions.Where(specification, parsed);
    }

    /// <summary>
    /// Orders the elements of the specified <see cref="Specification{T}"/> according to a specified property.
    /// <para>
    /// Example usage: "id asc" or "name desc"
    /// </para>
    /// </summary>
    /// <typeparam name="T">Data model type.</typeparam>
    /// <param name="specification">The <see cref="Specification{T}"/> to be ordered.</param>
    /// <param name="orderBy">The name of the property to order by. This must be a valid property name of the type <typeparamref name="T"/>.</param>
    /// <remarks>Use operators: asc or desc</remarks>
    /// <returns>A new <see cref="Specification{T}"/> with the elements ordered by the specified property.</returns>
    public static Specification<T> OrderBy<T>(
        this Specification<T> specification, string orderBy)
        => InnerOrderBy(specification, orderBy);

    /// <summary>
    /// Orders the elements of a specification based on a specified property.
    /// <para>
    /// Example usage: "id asc" or "name desc"
    /// </para>
    /// </summary>
    /// <typeparam name="T">Data model type.</typeparam>
    /// <typeparam name="R">Projected type.</typeparam>
    /// <param name="specification">The specification to be ordered.</param>
    /// <param name="orderBy">The name of the property to order by.</param>
    /// <remarks>Use operators: asc or desc</remarks>
    /// <returns>A new specification with elements ordered by the specified property.</returns>
    public static Specification<T, R> OrderBy<T, R>(
        this Specification<T, R> specification, string orderBy)
        => (Specification<T, R>)InnerOrderBy(specification, orderBy);

    private static Specification<T> InnerOrderBy<T>(
        this Specification<T> specification, string orderBy)
    {
        var parsed = OrderByExpressionParser.ParseOrderBy<T>(orderBy);

        return SpecificationExtensions.OrderBy<T>(specification, parsed.Item1, parsed.Item2);
    }
}

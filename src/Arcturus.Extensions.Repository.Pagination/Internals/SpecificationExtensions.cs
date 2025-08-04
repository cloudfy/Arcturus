using Arcturus.Repository.Specification;
using Arcturus.Repository.Specification.Parsing;

namespace Arcturus.Repository.Pagination.Internals;

internal static class SpecificationExtensions
{
    internal static Specification<T, R> WhereIfNotNull<T, R>(
        this Specification<T, R> spec
        , Expression<Func<T, bool>>? predicate)
    {
        if (predicate is not null)
            return spec.Where(predicate);
        return spec;
    }
    internal static Specification<T> WhereIfNotNull<T>(
        this Specification<T> spec
        , Expression<Func<T, bool>>? predicate)
    {
        if (predicate is not null)
            return spec.Where(predicate);
        return spec;
    }
    internal static Specification<T, R> WhereRange<T, R>(
        this Specification<T, R> spec
        , string[]? where)
    {
        if (where is not null && where.Length > 0)
        {
            foreach (var w in where)
            {
                spec.Where(w);
            }
        }
        return spec;
    }
    internal static Specification<T> WhereRange<T>(
        this Specification<T> spec
        , string[]? where)
    {
        if (where is not null && where.Length > 0)
        {
            foreach (var w in where)
            {
                spec.Where(w);
            }
        }
        return spec;
    }
}

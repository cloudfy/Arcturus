using System.Linq.Expressions;

namespace Arcturus.Data.Repository.Abstracts;

public static class SpecificationExtensions
{
    public static Specification<TEntity> AsSplitQuery<TEntity>(
        this Specification<TEntity> specification)
    {
        specification.UseSplitQuery = true;
        return specification;
    }
    public static IncludableSpecificationBuilder<TEntity, TProperty> Include<TEntity, TProperty>(
        this Specification<TEntity> spec,
        Expression<Func<TEntity, TProperty>> navigationPropertyPath)
        where TEntity : class
    {
        var builder = new IncludableSpecificationBuilder<TEntity, TProperty>(navigationPropertyPath);
        spec.AddIncludeChain(builder.IncludeChain);
        return builder;
    }

    public static IncludableSpecificationBuilder<TEntity, TNextProperty> ThenInclude<TEntity, TPreviousProperty, TNextProperty>(
        this IncludableSpecificationBuilder<TEntity, IEnumerable<TPreviousProperty>> source,
        Expression<Func<TPreviousProperty, TNextProperty>> navigationPropertyPath)
    {
        return new IncludableSpecificationBuilder<TEntity, TNextProperty>(source.IncludeChain, navigationPropertyPath);
    }

    public static IncludableSpecificationBuilder<TEntity, TNextProperty> ThenInclude<TEntity, TPreviousProperty, TNextProperty>(
        this IncludableSpecificationBuilder<TEntity, TPreviousProperty> source,
        Expression<Func<TPreviousProperty, TNextProperty>> navigationPropertyPath)
    {
        return new IncludableSpecificationBuilder<TEntity, TNextProperty>(source.IncludeChain, navigationPropertyPath);
    }

}


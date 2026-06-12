using Arcturus.Repository.Specification;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Arcturus.Repository.EntityFrameworkCore.InMemory.Internals;

/// <summary>
/// Evaluates and applies a given specification to an <see cref="IQueryable{TEntity}"/> source, transforming it
/// according to the specification's criteria and projections.
/// </summary>
/// <remarks>This evaluator processes include expressions, query splitting, and query filter ignoring as specified
/// by the <see cref="ISpecification{TEntity}"/>. It also handles projections if the specification includes a projection
/// for a different result type.</remarks>
/// <typeparam name="TEntity">The type of the entity being queried.</typeparam>
/// <param name="specification"></param>
internal sealed class SqlSpecificationEvaluator<TEntity>(
    ISpecification<TEntity> specification)
    : SpecificationEvaluator<TEntity>(specification)
    where TEntity : class
{
    public override IQueryable<TResult> Apply<TResult>(
        IQueryable<TEntity> source)
    {
        var result = Apply(source);
        if (Specification is ISpecification<TEntity, TResult> specificationWithOutput &&
            specificationWithOutput.Projection is not null)
        {
            return result.Select(specificationWithOutput.Projection!);
        }

        //// Try to cast if TResult is TEntity or a base type/interface
        //if (typeof(TResult).IsAssignableFrom(typeof(TEntity)))
        //{
        //    // This cast works if TResult is TEntity, or a base class/interface of TEntity
        //    return (IQueryable<TResult>)result;
        //}

        throw new InvalidOperationException(
            $"Cannot cast {typeof(TEntity)} to {typeof(TResult)} and no projection is specified."
    );
    }

    public override IQueryable<TEntity> Apply(
        IQueryable<TEntity> source)
    {
        // Get all include paths and remove redundant parent paths
        var allPaths = Specification.IncludeExpressions
            .SelectMany(_ => _.Chains)
            .Distinct()
            .ToList();

        // Remove paths that are prefixes of other paths
        var filteredPaths = allPaths
            .Where(path => !allPaths.Any(otherPath =>
                otherPath != path &&
                otherPath.StartsWith(path + ".")))
            .OrderBy(chain => chain.Length)
            .ThenBy(chain => chain)
            .ToList();

        if (filteredPaths.Count > 0)
        {
            foreach (var chain in filteredPaths)
            {
                source = source.Include(chain);
            }
        }

        if (Specification.UseSplitQuery)
        {
            // In-memory provider does not support split queries, so this is a no-op.
        }
        if (Specification.IgnoreQueryFilters)
        {
            source = source.IgnoreQueryFilters();
        }

        return ApplyLimit(ApplySkip(ApplyWhere(ApplyOrderBy(source))));
    }

    private static IQueryable<TEntity> ApplyIncludeChain(IQueryable<TEntity> source, List<string> chain)
    {
        if (chain.Count == 0) return source;

        var query = EntityFrameworkQueryableExtensions.Include(source, (dynamic)chain[0]);

        for (int i = 1; i < chain.Count; i++)
        {
            query = EntityFrameworkQueryableExtensions.ThenInclude((dynamic)query, (dynamic)chain[i]);
        }

        return query;
    }
}

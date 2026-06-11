namespace Arcturus.Repository.Specification;

/// <summary>
/// Defines a builder interface for constructing include paths in a specification. This interface is used to facilitate
/// the inclusion of related entities in a query.
/// </summary>
/// <typeparam name="TEntity">The type of the entity being queried.</typeparam>
/// <typeparam name="TBranchRoot">The type of the root entity in the current include branch.</typeparam>
/// <typeparam name="TCurrent">The type of the current entity in the include path.</typeparam>
public interface IIncludeBuilder<TEntity, TBranchRoot, TCurrent>
{
    /// <summary>
    /// Gets the specification associated with this include builder. This specification contains the query criteria and
    /// </summary>
    Specification<TEntity> Specification { get; }
    /// <summary>
    /// Gets the root path of the include chain. This represents the initial navigation property from which the include
    /// </summary>
    string RootPath { get; }
    /// <summary>
    /// Gets the current path of the include chain. This represents the navigation property path up to the current point in
    /// </summary>
    string CurrentPath { get; }
}

namespace Arcturus.Repository.Specification;

internal sealed class IncludeBuilder<TEntity, TBranchRoot, TCurrent>
    : IIncludeBuilder<TEntity, TBranchRoot, TCurrent>
{
    internal IncludeBuilder(
        Specification<TEntity> specification,
        string rootPath,
        string currentPath)
    {
        Specification = specification;
        RootPath = rootPath;
        CurrentPath = currentPath;
    }

    public Specification<TEntity> Specification { get; }

    public string RootPath { get; }

    public string CurrentPath { get; }

    public static implicit operator Specification<TEntity>(
        IncludeBuilder<TEntity, TBranchRoot, TCurrent> builder)
            => builder.Specification;
}

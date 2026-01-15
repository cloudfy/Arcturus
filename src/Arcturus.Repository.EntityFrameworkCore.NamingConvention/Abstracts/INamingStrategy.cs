namespace Arcturus.Repository.EntityFrameworkCore.NamingConvention.Abstracts;

/// <summary>
/// Defines a strategy for applying a specific naming convention to string values,
/// such as entity, table, or column names.
/// </summary>
public interface INamingStrategy
{
    /// <summary>
    /// Applies the naming convention defined by this strategy to the specified name.
    /// </summary>
    /// <param name="name">The original name to which the naming convention should be applied.</param>
    /// <returns>The transformed name after applying the naming convention.</returns>
    string ApplyNaming(string name);
}
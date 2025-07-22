namespace Arcturus.Repository.Abstracts;

/// <summary>
/// Represents an entity with a unique identifier.
/// </summary>
/// <typeparam name="TKey">
/// The type of the unique identifier for the entity. Must be non-nullable.
/// </typeparam>
public interface IEntity<TKey> where TKey : notnull
{
    /// <summary>
    /// Gets the unique identifier for the entity.
    /// </summary>
    TKey Id { get; }
}
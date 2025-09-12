namespace Arcturus.Repository.Abstracts;

/// <summary>
/// Represents the result of an add or update operation in a data store.
/// </summary>
/// <remarks>This structure provides information about whether an existing entity was updated,  the number of rows
/// affected by the operation, and the entity that was inserted, if applicable.</remarks>
/// <typeparam name="T">The type of the entity involved in the operation.</typeparam>
/// <param name="Updated">Indicates whether an existing entity was updated (true) or a new entity was inserted (false).</param>
/// <param name="RowsAffected">The number of rows affected by the add or update operation.</param>
/// <param name="InsertedEntity">The entity that was inserted, or null if an existing entity was updated.</param>
public readonly record struct AddOrUpdateResult<T>(
    bool Updated, int RowsAffected, T? InsertedEntity);


namespace Arcturus.Repository.Abstracts;

/// <summary>
/// Represents the result of an add or update operation in a data store.
/// </summary>
/// <remarks>This structure provides information about whether an existing entity was updated,  the number of rows
/// affected by the operation, and the entity that was inserted, if applicable.</remarks>
/// <typeparam name="T">The type of the entity involved in the operation.</typeparam>
/// <param name="Updated"></param>
/// <param name="RowsAffected"></param>
/// <param name="InsertedEntity"></param>
public readonly record struct AddOrUpdateResult<T>(
    bool Updated, int RowsAffected, T? InsertedEntity);


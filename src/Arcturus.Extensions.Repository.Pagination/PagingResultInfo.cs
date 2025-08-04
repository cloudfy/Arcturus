namespace Arcturus.Repository.Pagination;

/// <summary>
/// Represents information about the result of a paginated query, including a cursor for fetching subsequent pages.
/// </summary>
/// <remarks>This record is typically used in APIs that support pagination, allowing clients to request the next
/// set of results by providing the <see cref="After"/> cursor in subsequent requests.</remarks>
/// <param name="After">A cursor or token indicating the position after which the next set of results should be fetched.</param>
public record PagingResultInfo(string? After);
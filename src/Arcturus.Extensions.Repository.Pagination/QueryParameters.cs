namespace Arcturus.Repository.Pagination;

/// <summary>
/// Represents a set of parameters used to query data with optional filtering, ordering, and pagination.
/// </summary>
/// <remarks>This record is typically used to encapsulate query options for data retrieval operations.</remarks>
/// <param name="After">An optional cursor indicating the starting point for the query. If specified, the query will return results after
/// this cursor.</param>
/// <param name="Before">An optional cursor indicating the ending point for the query. If specified, the query will return results before
/// this cursor.</param>
/// <param name="Filter">An optional array of filter expressions to apply to the query. Each expression should be a valid filter condition.</param>
/// <param name="OrderBy">An optional array of fields by which to order the query results. The order of fields in the array determines the
/// order of sorting.</param>
/// <param name="Limit">An optional maximum number of results to return. If specified, the query will return no more than this number of
/// results.</param>
/// <param name="Page">An optional page number for paginated results. The <paramref name="Page"/> does not have any logic, but is used as 
/// a placeholder to help context. Helper field only.</param>
public record QueryParameters
    (string? After, string[]? Filter, string[]? OrderBy, int? Limit, string? Before = null, int? Page = null)
{
}
namespace Arcturus.Repository.Pagination;

/// <summary>
/// Represents a paginated list of results, including metadata about the total count, paging state, and whether more
/// results are available.
/// </summary>
/// <remarks>This class is designed to encapsulate a subset of results from a larger data set, along with metadata
/// such as the total count of items, the number of pages, and whether additional results are available. It is typically
/// used in scenarios where data is retrieved in pages or chunks.</remarks>
/// <typeparam name="TResult">The type of the elements in the result set.</typeparam>
public sealed class PagingResultsList<TResult>
{
    private readonly int _limit;
    private readonly string? _after;

    internal PagingResultsList(
        TResult[] results
        , long totalCount
        , int limit
        , string? after)
    {
        _limit = limit;
        TotalCount = totalCount;

        if (results.Length < totalCount && totalCount > 0 && results.Length > limit)
        {
            Results = results[..^1];
            HasMore = true;
            _after = after;
        }
        else
        {
            Results = results;
        }
    }
    /// <summary>
    /// Gets the total count of the full data set.
    /// </summary>
    public long TotalCount { get; private set; } = 0;
    /// <summary>
    /// Gets the number of elements in the <see cref="Results"/> array.
    /// </summary>
    public long Count => Results.Length;
    /// <summary>
    /// Gets the total number of pages based on the total count of items and the limit per page.
    /// </summary>
    public long Pages => TotalCount == 0 ? 0 : 1 + TotalCount / _limit;
    /// <summary>
    /// // Gets a value indicating whether there are more results available beyond the current page.
    /// </summary>
    public bool HasMore { get; private set; } = false;
    /// <summary>
    /// Gets the array of results produced by the request.
    /// </summary>
    public TResult[] Results { get; private set; } = [];
    /// <summary>
    /// Gets the paging information for the current result set.
    /// </summary>
    public PagingResultInfo? Paging
    {
        get
        {
            if (_after is null)
                return null;
            return new PagingResultInfo(_after.ToString());
        }
    }
    /// <summary>
    /// Creates an empty instance of <see cref="PagingResultsList{TResult}"/> with no items and default pagination
    /// values.
    /// </summary>
    /// <returns>An empty <see cref="PagingResultsList{TResult}"/> with an empty item list, a total count of 0,  and no
    /// additional metadata.</returns>
    public static PagingResultsList<TResult> Empty()
    {
        return new([], 0, 0, null);
    }
}

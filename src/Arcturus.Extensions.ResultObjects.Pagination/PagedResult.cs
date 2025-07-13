using System.Text.Json.Serialization;

namespace Arcturus.ResultObjects.Pagination;

public sealed class PagedResult<T, TKey>
{
    internal PagedResult(T[] results, long totalCount,TKey? nextToken, int pageSize = 20)
    {
        Results = [.. results.Take(pageSize)];
        TotalCount = totalCount;
        HasMore = results.Length > pageSize;
        NextToken = nextToken;
    }
    public T[] Results { get; } = [];
    public long TotalCount { get; }
    public bool HasMore { get; }
    public TKey? NextToken { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Dictionary<string, string>? Links { get; set; }
}
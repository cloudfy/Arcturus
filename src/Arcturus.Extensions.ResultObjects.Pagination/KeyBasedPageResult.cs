namespace Arcturus.ResultObjects.Pagination;

public sealed class KeyBasedPageResult<T, TKey> 
    : Result<PagedResult<T, TKey>>
{
    internal KeyBasedPageResult(
        bool isSuccess, PagedResult<T, TKey> pagedResult, Fault? fault = null) 
        : base(isSuccess, pagedResult, fault)
    {
    }

    public static KeyBasedPageResult<T, TKey> FromArray(
        T[] results, long totalCount, TKey? nextToken, int pageSize = 20)
    {
        var value = new PagedResult<T, TKey>(results, totalCount, nextToken, pageSize);
        return new KeyBasedPageResult<T, TKey>(true, value);
    }
}

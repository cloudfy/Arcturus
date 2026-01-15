using Arcturus.Extensions.ResultObjects.AspNetCore.Internals;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace Arcturus.Extensions.ResultObjects.AspNetCore.Results;

public sealed class ObjectResult<TValue> 
    : IResult, IStatusCodeHttpResult, IValueHttpResult, IValueHttpResult<TValue>
{
    internal ObjectResult(TValue? value, HttpStatusCode httpStatusCode)
    {
        Value = value;
        StatusCode = (int)httpStatusCode;
    }

    public TValue? Value { get; }
    public int StatusCode { get; }

    int? IStatusCodeHttpResult.StatusCode => StatusCode;

    object? IValueHttpResult.Value => Value;

    public Task ExecuteAsync(HttpContext httpContext)
    {
        ArgumentNullException.ThrowIfNull(httpContext);

        httpContext.Response.StatusCode = StatusCode;

        return HttpResultsHelper.WriteResultAsJsonAsync(
            httpContext
            , Value);
    }
}
public sealed class ObjectResult : IResult, IStatusCodeHttpResult, IValueHttpResult
{
    internal ObjectResult(object? value, HttpStatusCode httpStatusCode)
    {
        Value = value;
        StatusCode = (int)httpStatusCode;
    }

    public object? Value { get; }
    public int StatusCode { get; }

    int? IStatusCodeHttpResult.StatusCode => StatusCode;

    object? IValueHttpResult.Value => Value;

    public Task ExecuteAsync(HttpContext httpContext)
    {
        ArgumentNullException.ThrowIfNull(httpContext);

        httpContext.Response.StatusCode = StatusCode;

        return HttpResultsHelper.WriteResultAsJsonAsync(
            httpContext
            , Value);
    }
}

using System.Net;

namespace Arcturus.ResultObjects;

public static class ResultExtensions
{
    public static Result<T> WithException<T>(this Result<T> value, Exception exception)
    {
        value.Exception = exception;
        value.IsSuccess = false;
        return value;
    }
    public static Result<T> WithHttpStatusCode<T>(this Result<T> value, HttpStatusCode httpStatusCode)
    {
        value.HttpStatusCode = httpStatusCode;
        return value;
    }
}

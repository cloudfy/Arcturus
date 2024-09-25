using System.Net;

namespace Arcturus.ResultObjects;

public static class ResultExtensions
{
    /// <summary>
    /// Assigns the <paramref name="exception"/> to the result.
    /// <para>
    /// <b>Result object will be marked as failure.</b>
    /// </para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value">Required.</param>
    /// <param name="exception"></param>
    /// <returns><see cref="Result{T}"/></returns>
    public static Result<T> WithException<T>(this Result<T> value, Exception exception)
    {
        value.Exception = exception;
        value.IsSuccess = false;
        return value;
    }
    /// <summary>
    /// Assigns the <paramref name="httpStatusCode"/> to the result.
    /// </summary>
    /// <typeparam name="T">Type of result object.</typeparam>
    /// <param name="value">Required.</param>
    /// <param name="httpStatusCode"><see cref="HttpStatusCode"/> to assign.</param>
    /// <returns><see cref="Result{T}"/></returns>
    public static Result<T> WithHttpStatusCode<T>(this Result<T> value, HttpStatusCode httpStatusCode)
    {
        value.HttpStatusCode = httpStatusCode;
        return value;
    }
}

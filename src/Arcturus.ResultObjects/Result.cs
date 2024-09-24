using System.Net;

namespace Arcturus.ResultObjects;

/// <summary>
/// 
/// </summary>
public class Result
{
    internal Result(bool isSuccess) => IsSuccess = isSuccess;
    
    /// <summary>
    /// Returns a success result.
    /// </summary>
    /// <returns><see cref="Result"/></returns>
    public static Result Success() => new(true);
    /// <summary>
    /// Returns a success result with a <paramref name="value"/>.
    /// </summary>
    /// <typeparam name="TValue">Type of value.</typeparam>
    /// <param name="value">Optional. Value of result.</param>
    /// <returns><see cref="Result{T}"/></returns>
    public static Result<TValue> Success<TValue>(TValue? value) => new(true, value);
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static Result Failure() => new(false);

    /// <summary>
    /// Returns true if the result is a success.
    /// </summary>
    public bool IsSuccess { get; internal set; }
    /// <summary>
    /// Returns true if the result is not a success.
    /// </summary>
    public bool IsFailure => !IsSuccess;
    public Exception? Exception { get; internal set; }
    public HttpStatusCode? HttpStatusCode { get; internal set; }
}

using System.Net;

namespace Arcturus.ResultObjects;

/// <summary>
/// Represents a result of an operation - success or failure.
/// </summary>
public class Result
{
    internal Result(bool isSuccess, Fault? fault = null)
    {
        IsSuccess = isSuccess;
        Fault = fault;
    }
    
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
    /// Returns a failure result with an optional <paramref name="fault"/>.
    /// </summary>
    /// <param name="fault">Optional.</param>
    /// <returns><see cref="Result"/></returns>
    public static Result Failure(Fault? fault = null) => new(false, fault);
    /// <summary>
    /// Returns a failure result with an optional <paramref name="fault"/>.
    /// </summary>
    /// <typeparam name="TValue">Type of value.</typeparam>
    /// <param name="fault">Optional.</param>
    /// <returns><see cref="Result{T}"/></returns>
    public static Result<TValue> Failure<TValue>(Fault? fault = null) => new(false, default, fault);
    /// <summary>
    /// Returns true if the result is a success.
    /// </summary>
    public bool IsSuccess { get; internal set; }
    /// <summary>
    /// Returns true if the result is not a success.
    /// </summary>
    public bool IsFailure => !IsSuccess;
    /// <summary>
    /// Gets an exception if assigned. Use <see cref="ResultExtensions.WithException{T}(Result{T}, Exception)"/>.
    /// </summary>
    public Exception? Exception { get; internal set; }
    /// <summary>
    /// Gets the reason for the failure when <see cref="IsFailure"/> is true.
    /// </summary>
    public Fault? Fault { get; }
    /// <summary>
    /// Gets an HttpStatusCode if assigned. Use <see cref="ResultExtensions.WithHttpStatusCode{T}(Result{T}, HttpStatusCode)"/>.
    /// </summary>
    public HttpStatusCode? HttpStatusCode { get; internal set; }
}

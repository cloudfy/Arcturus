using Arcturus.ResultObjects.Specialized;
using System.Net;

namespace Arcturus.ResultObjects;

/// <summary>
/// Represents a result of an operation - success or failure.
/// </summary>
public class Result
{
    /// <summary>
    /// Creates a new instance of <see cref="Result"/> with a <paramref name="fault"/> and <paramref name="isSuccess"/>.
    /// </summary>
    /// <param name="isSuccess">A value of <see cref="bool"/> indicating is the result represens success.</param>
    /// <param name="fault">Optional <see cref="ResultObjects.Fault"/> if <paramref name="isSuccess"/> is false.</param>
    protected Result(bool isSuccess, Fault? fault = null)
    {
        IsSuccess = isSuccess;
        Fault = fault;
        if (fault is ISpecializedFault specializedFault)
        {
            HttpStatusCode = specializedFault.HttpStatusCode;
        }
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
    public static Result<TValue> Success<TValue>(TValue? value) => Result<TValue>.Create(true, value);
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
    public static Result<TValue> Failure<TValue>(Fault? fault = null) => Result<TValue>.Create(false, default, fault);
    /// <summary>
    /// Returns a failure result with a fault of <paramref name="code"/> and <paramref name="message"/>
    /// </summary>
    /// <typeparam name="TValue">Type of value.</typeparam>
    /// <param name="code">Optional code.</param>
    /// <param name="message">Message of the fault.</param>
    /// <returns><see cref="Result{T}"/></returns>
    public static Result<TValue> Failure<TValue>(string? code, string message) => Result<TValue>.Create(false, default, new Fault(code, message));
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
    /// <summary>
    /// Gets a url of a help documentation.
    /// </summary>
    public string? HelpLink { get; internal set; }
}

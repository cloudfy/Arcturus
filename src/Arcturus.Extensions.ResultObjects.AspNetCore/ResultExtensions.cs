using Microsoft.AspNetCore.Mvc;
using System.Net;
using Arcturus.Extensions.ResultObjects.AspNetCore.ActionResults;
using Arcturus.Extensions.ResultObjects.AspNetCore.Results;

namespace Arcturus.ResultObjects;

public static class ResultExtensions
{
    private static HttpStatusCode _defaultStatusCode = HttpStatusCode.BadRequest;
    
    /// <summary>
    /// Converts a <see cref="Result{T}"/> object to an <see cref="IActionResult"/> object.
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <param name="result">The <see cref="Result{T}"/> object to convert.</param>
    /// <returns>An <see cref="IActionResult"/> object representing the result.</returns>
    public static IActionResult ToActionResult<T>(this Result<T> result)
    {
        if (result.IsSuccess && result.HttpStatusCode is null)
            return new OkObjectResult(result.Value);

        if (result.IsSuccess && result.HttpStatusCode is not null)
            return new OkObjectResult(result.Value) { StatusCode = (int)result.HttpStatusCode.Value };

        if (result.HttpStatusCode is not null)
            return new ProblemDetailsActionResult(result);

        //return new Microsoft.AspNetCore.Mvc.ObjectResult(null) { StatusCode = 500 };
        return new ProblemDetailsActionResult(
            result.WithHttpStatusCode(_defaultStatusCode));
    }
    /// <summary>
    /// Converts a <see cref="Result"/> object to an <see cref="IActionResult"/> object.
    /// </summary>
    /// <param name="result">The <see cref="Result"/> object to convert.</param>
    /// <returns>An <see cref="IActionResult"/> object representing the result.</returns>
    public static IActionResult ToActionResult(this Result result)
    {
        if (result.IsSuccess && result.HttpStatusCode is null)
            return new OkObjectResult(null);

        if (result.IsSuccess && result.HttpStatusCode is not null)
            return new OkObjectResult(null) { StatusCode = (int)result.HttpStatusCode.Value };

        if (result.HttpStatusCode is not null)
            return new ProblemDetailsActionResult(result);

        // return new Microsoft.AspNetCore.Mvc.ObjectResult(null) { StatusCode = 500 };
        return new ProblemDetailsActionResult(
            result.WithHttpStatusCode(_defaultStatusCode));

    }
    /// <summary>
    /// Converts a Task of <see cref="Result"/> object to an <see cref="IActionResult"/> object.
    /// </summary>
    /// <param name="result">The <see cref="Result"/> object to convert.</param>
    /// <returns>An <see cref="IActionResult"/> object representing the result.</returns>
    public async static Task<IActionResult> ToActionResult(this Task<Result> result)
        => (await result).ToActionResult();
    /// <summary>
    /// Converts a <see cref="Result{T}"/> object to an <see cref="IActionResult"/> object.
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <param name="result">The <see cref="Result{T}"/> object to convert.</param>
    /// <returns>An <see cref="IActionResult"/> object representing the result.</returns>
    public async static Task<IActionResult> ToActionResult<T>(this Task<Result<T>> result)
        => (await result).ToActionResult();
    /// <summary>
    /// Converts a <see cref="Result{T}"/> object to an <see cref="Microsoft.AspNetCore.Http.IResult"/> object.
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <param name="result">The <see cref="Result{T}"/> object to convert.</param>
    /// <returns>An <see cref="Microsoft.AspNetCore.Http.IResult"/> object representing the result.</returns>
    public static Microsoft.AspNetCore.Http.IResult ToResult<T>(this Result<T> result)
    {
        if (result.IsSuccess && result.HttpStatusCode is null)
            return Microsoft.AspNetCore.Http.Results.Ok(result.Value);

        if (result.IsSuccess && result.HttpStatusCode is not null)
            return new ObjectResult<T>(result.Value, result.HttpStatusCode.Value);
 
        if (result.HttpStatusCode is not null)
            return new ProblemDetailsResult(result);

        //return Microsoft.AspNetCore.Http.Results.Problem();
        return new ProblemDetailsResult(
            result.WithHttpStatusCode(_defaultStatusCode));
    }
    /// <summary>
    /// Converts a <see cref="Result{T}"/> object to an <see cref="Microsoft.AspNetCore.Http.IResult"/> object.
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <param name="taskResult">The <see cref="Result{T}"/> object to convert.</param>
    /// <returns>An <see cref="Microsoft.AspNetCore.Http.IResult"/> object representing the result.</returns>
    public async static Task<Microsoft.AspNetCore.Http.IResult> ToResult<T>(this Task<Result<T>> taskResult)
        => ToResult<T>(await taskResult);

    /// <summary>
    /// Converts a <see cref="Result"/> instance into an ASP.NET Core <see cref="Microsoft.AspNetCore.Http.IResult"/>.
    /// </summary>
    /// <param name="result">The <see cref="Result"/> instance to convert. Must not be <c>null</c>.</param>
    /// <param name="methodDelegate"></param>
    /// <returns>An <see cref="Microsoft.AspNetCore.Http.IResult"/> representing the outcome of the <paramref name="result"/>:
    /// <list type="bullet"> <item><description>If <paramref name="result"/> is successful and has no HTTP status code,
    /// returns an <see cref="Microsoft.AspNetCore.Http.Results.Ok"/> result.</description></item> <item><description>If
    /// <paramref name="result"/> is successful and has an HTTP status code, returns an <see
    /// cref="Extensions.ResultObjects.AspNetCore.Results.ObjectResult"/> with the specified status
    /// code.</description></item> <item><description>If <paramref name="result"/> is unsuccessful and has an HTTP
    /// status code, returns a <see cref="ProblemDetailsResult"/> with the specified status code.</description></item>
    /// <item><description>If <paramref name="result"/> is unsuccessful and has no HTTP status code, returns a <see
    /// cref="ProblemDetailsResult"/> with a default status code.</description></item> </list></returns>
    public static Microsoft.AspNetCore.Http.IResult ToResult(
        this Result result
        , Func<Result, object?, Microsoft.AspNetCore.Http.IResult>? methodDelegate = null)
    {
        object? value = null;
        if (result is IResultValue resultWithValue)
        {
            value = resultWithValue.Value;
        }

        if (methodDelegate is not null && result.IsSuccess)
            return methodDelegate(result, value);

        if (result.IsSuccess && result.HttpStatusCode is null)
            return Microsoft.AspNetCore.Http.Results.Ok(value);

        if (result.IsSuccess && result.HttpStatusCode is not null)
            return new Extensions.ResultObjects.AspNetCore.Results.ObjectResult(value, result.HttpStatusCode.Value);
        
        if (result.HttpStatusCode is not null)
            return new ProblemDetailsResult(result);

        return new ProblemDetailsResult(
            result.WithHttpStatusCode(_defaultStatusCode));
    }
    /// <summary>
    /// Converts a <see cref="Task{TResult}"/> of type <see cref="Result"/> into an <see cref="Microsoft.AspNetCore.Http.IResult"/>.
    /// </summary>
    /// <remarks>This method awaits the provided <paramref name="result"/> task and converts the resulting 
    /// <see cref="Result"/> into an <see cref="Microsoft.AspNetCore.Http.IResult"/>. It is useful for integrating  <see cref="Result"/> objects
    /// with ASP.NET Core's endpoint response system.</remarks>
    /// <param name="result">A task that represents the asynchronous operation returning a <see cref="Result"/>.</param>
    /// <param name="successDelegate">Provides a delegate which convert the Result is IsSuccess and Value to a <see cref="Microsoft.AspNetCore.Http.IResult"/>.</param>
    /// <returns>A task that represents the asynchronous operation, producing an <see cref="Microsoft.AspNetCore.Http.IResult"/>  corresponding to the
    /// outcome of the <see cref="Result"/>.</returns>
    public async static Task<Microsoft.AspNetCore.Http.IResult> ToResult(
        this Task<Result> result
        , Func<Result, object?, Microsoft.AspNetCore.Http.IResult>? successDelegate = null)
        => (await result).ToResult(successDelegate);
}
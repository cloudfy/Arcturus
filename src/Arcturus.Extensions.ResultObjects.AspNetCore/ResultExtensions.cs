using Microsoft.AspNetCore.Mvc;

namespace Arcturus.ResultObjects;

public static class ResultExtensions
{
    /// <summary>
    /// Converts a <see cref="Result{T}"/> object to an <see cref="IActionResult"/> object.
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <param name="result">The <see cref="Result{T}"/> object to convert.</param>
    /// <returns>An <see cref="IActionResult"/> object representing the result.</returns>
    public static IActionResult ToActionResult<T>(this Result<T> result)
    {
        if (result.IsSuccess)
            return new OkObjectResult(result.Value);

        if (result.HttpStatusCode is not null)
            return new StatusCodeActionResult(result);

        return new ObjectResult(null) { StatusCode = 500 };
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="result"></param>
    /// <returns></returns>
    public static IActionResult ToActionResult(this Result result)
    {
        if (result.IsSuccess)
            return new OkObjectResult(null);

        if (result.HttpStatusCode is not null)
            return new StatusCodeActionResult(result);

        return new ObjectResult(null) { StatusCode = 500 };
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="result"></param>
    /// <returns></returns>
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
        if (result.IsSuccess)
            return Microsoft.AspNetCore.Http.Results.Ok(result.Value);

        if (result.HttpStatusCode is not null)
            return new StatusCodeResult(result);

        return Microsoft.AspNetCore.Http.Results.Problem();
    }
    /// <summary>
    /// Converts a <see cref="Result{T}"/> object to an <see cref="Microsoft.AspNetCore.Http.IResult"/> object.
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <param name="result">The <see cref="Result{T}"/> object to convert.</param>
    /// <returns>An <see cref="Microsoft.AspNetCore.Http.IResult"/> object representing the result.</returns>
    public async static Task<Microsoft.AspNetCore.Http.IResult> ToResult<T>(this Task<Result<T>> result)
        => (await result).ToResult();
    /// <summary>
    /// 
    /// </summary>
    /// <param name="result"></param>
    /// <returns></returns>
    public static Microsoft.AspNetCore.Http.IResult ToResult(this Result result)
    {
        if (result.IsSuccess)
            return Microsoft.AspNetCore.Http.Results.Ok();

        if (result.HttpStatusCode is not null)
            return new StatusCodeResult(result);

        return Microsoft.AspNetCore.Http.Results.Problem();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="result"></param>
    /// <returns></returns>
    public async static Task<Microsoft.AspNetCore.Http.IResult> ToResult(this Task<Result> result)
       => (await result).ToResult();
}

using Microsoft.AspNetCore.Mvc;

namespace Arcturus.ResultObjects;

public static class ResultExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="result"></param>
    /// <returns></returns>
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
    /// <typeparam name="T"></typeparam>
    /// <param name="result"></param>
    /// <returns></returns>
    public async static Task<IActionResult> ToActionResult<T>(this Task<Result<T>> result)
        => (await result).ToActionResult();
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="result"></param>
    /// <returns></returns>
    public static Microsoft.AspNetCore.Http.IResult ToResult<T>(this Result<T> result)
    {
        if (result.IsSuccess)
            return Microsoft.AspNetCore.Http.Results.Ok(result.Value);

        if (result.HttpStatusCode is not null)
            return new StatusCodeResult(result);

        return Microsoft.AspNetCore.Http.Results.Problem();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="result"></param>
    /// <returns></returns>
    public async static Task<Microsoft.AspNetCore.Http.IResult> ToResult<T>(this Task<Result<T>> result)
        => (await result).ToResult();
}

﻿using Microsoft.AspNetCore.Mvc;
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
        if (result.IsSuccess && result.HttpStatusCode is null)
            return Microsoft.AspNetCore.Http.Results.Ok();

        if (result.IsSuccess && result.HttpStatusCode is not null)
            return new Extensions.ResultObjects.AspNetCore.Results.ObjectResult(null, result.HttpStatusCode.Value);

        if (result.HttpStatusCode is not null)
            return new ProblemDetailsResult(result);

//        return Microsoft.AspNetCore.Http.Results.Problem();
        return new ProblemDetailsResult(
            result.WithHttpStatusCode(_defaultStatusCode));
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="result"></param>
    /// <returns></returns>
    public async static Task<Microsoft.AspNetCore.Http.IResult> ToResult(this Task<Result> result)
       => (await result).ToResult();
}

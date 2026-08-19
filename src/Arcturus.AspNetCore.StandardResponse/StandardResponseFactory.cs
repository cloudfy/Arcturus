using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Arcturus.AspNetCore.StandardResponse;

public interface IStandardResponseFactory
{
    ProblemDetails CreateProblemDetailsResponse(
        string? title, string? detail, string? instance, HttpStatusCode statusCode = HttpStatusCode.InternalServerError);
    ProblemDetails CreateDefaultProblemDetailsResponse(HttpContext httpContext);
    Task WriteResponse(HttpContext httpContext, ProblemDetails problemDetails, Exception? exception = null);
}
public sealed class StandardResponseFactory(
    IProblemDetailsService problemDetailsService) : IStandardResponseFactory
{
    private readonly IProblemDetailsService _problemDetailsService = problemDetailsService;

    public ProblemDetails CreateProblemDetailsResponse(
        string? title, string? detail, string? instance, HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
    {
        return new ProblemDetails
        {
            Status = (int)statusCode
            , Title = title
            , Detail = detail
            , Instance = instance
        };
    }
    public ProblemDetails CreateDefaultProblemDetailsResponse(HttpContext httpContext)
    {
        return new ProblemDetails
        {
            Status = 500
            , Title = "An unhandled exception occurred."
            , Detail = "An unhandled error occurred. Staff have been notified. Please use the traceId for reference."
            , Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}"
        };
    }

    public async Task WriteResponse(HttpContext httpContext, ProblemDetails problemDetails, Exception? exception = null)
    {
        ProblemDetailDefaults.ApplyDefaults(problemDetails, null, httpContext, exception);

        if (_problemDetailsService is not null)
        {
            await _problemDetailsService.WriteAsync(new ProblemDetailsContext
            {
                HttpContext = httpContext
                , Exception = exception
                , ProblemDetails = problemDetails
            });
        }
        else
        {
            await FallbackProblemDetailsWriter.Write(problemDetails, httpContext);
        }
    }
}

using Arcturus.ResultObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Arcturus.Extensions.ResultObjects.AspNetCore.Internals;

internal static class ProblemDetailsFactory
{
    internal static ProblemDetails Create(Result result, HttpContext httpContext)
    {
        HttpStatusCode statusCode = result.HttpStatusCode ?? HttpStatusCode.BadRequest;

        var problemDetails = new ProblemDetails()
        {
            Status = (int)statusCode,
            Title = result.Fault?.Code,
            Detail = result.Fault?.Message,
            Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}"
        };

        return problemDetails;
    }
}

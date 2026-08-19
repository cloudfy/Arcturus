using Arcturus.AspNetCore.StandardResponse;
using Arcturus.ResultObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace Arcturus.Extensions.ResultObjects.AspNetCore.Results;

internal sealed class ProblemDetailsResult : IResult
{
    private readonly Result _result;

    internal ProblemDetailsResult(Result result) => _result = result;

    public async Task ExecuteAsync(HttpContext httpContext)
    {
        //var problemDetails = Internals.ProblemDetailsFactory.Create(_result, httpContext);
        //ProblemDetailDefaults.ApplyDefaults(problemDetails, _result, httpContext);

        var standardResponseFactory = httpContext.RequestServices.GetRequiredService<StandardResponseFactory>();
        var problemDetails = Create(_result, httpContext);

        await standardResponseFactory.WriteResponse(httpContext, problemDetails, _result.Exception);
    }

    private static ProblemDetails Create(Result result, HttpContext httpContext)
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

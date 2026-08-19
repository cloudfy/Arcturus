
using Arcturus.AspNetCore.StandardResponse;
using Arcturus.Extensions.ResultObjects.AspNetCore.Internals;
using Arcturus.ResultObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace Arcturus.Extensions.ResultObjects.AspNetCore.ActionResults;

public sealed class ProblemDetailsActionResult : IActionResult
{
    private readonly Result _result;

    internal ProblemDetailsActionResult(Result result) => _result = result;

    public Task ExecuteResultAsync(ActionContext context)
    {
        var problemDetails = Create(_result, context.HttpContext);

        var responseFactory = context.HttpContext.RequestServices.GetRequiredService<IStandardResponseFactory>();
        return responseFactory.WriteResponse(context.HttpContext, problemDetails, _result.Exception);
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
using Arcturus.Extensions.ResultObjects.AspNetCore.Internals;
using Arcturus.ResultObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace Arcturus.Extensions.ResultObjects.AspNetCore.Results;

public sealed class ProblemDetailsResult : IResult
{
    private readonly Result _result;

    internal ProblemDetailsResult(Result result) => _result = result;

    public Task ExecuteAsync(HttpContext httpContext)
    {
        HttpStatusCode defaultStatusCode = _result.HttpStatusCode ?? HttpStatusCode.BadRequest;

        var factory = httpContext.RequestServices.GetService<ProblemDetailsFactory>();
        if (factory is null)
            throw new ArgumentNullException("ProblemDetailsFactory is not registered in the service collection.");

        var problemDetails = factory!.CreateProblemDetails(httpContext, (int)defaultStatusCode);

        problemDetails.Title ??= _result.Fault?.Code;
        problemDetails.Detail = _result.Fault?.Message;
        problemDetails.Type ??= "https://schemas/2022/fault/#" + defaultStatusCode.ToString().ToLower();
        problemDetails.Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}";

        httpContext.Response.StatusCode = (int)defaultStatusCode;
        return HttpResultsHelper.WriteResultAsJsonAsync(
            httpContext
            , problemDetails);
    }
}

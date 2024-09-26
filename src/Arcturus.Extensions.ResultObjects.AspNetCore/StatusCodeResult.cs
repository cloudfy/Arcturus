using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace Arcturus.ResultObjects;

public sealed class StatusCodeResult : IResult
{
    private readonly Result _result;

    internal StatusCodeResult(Result result) => _result = result;
    
    public async Task ExecuteAsync(HttpContext httpContext)
    {
        HttpStatusCode defaultStatusCode = _result.HttpStatusCode ?? HttpStatusCode.BadRequest;

        //var factory1 = httpContext.RequestServices.GetService<IProblemDetailsService>();
        var factory = httpContext.RequestServices.GetService<ProblemDetailsFactory>();
        var problemDetails = factory!.CreateProblemDetails(httpContext, (int)defaultStatusCode);

        problemDetails.Title ??= _result.Fault?.Code;
        problemDetails.Detail = _result.Fault?.Message;

        //var cts = new ProblemDetailsContext()
        //{
        //    HttpContext = httpContext
        //};
        //cts.ProblemDetails.Detail = _result.Fault?.Message;
        //cts.ProblemDetails.Title = cts.ProblemDetails.Title ?? _result.Fault?.Code;
        //cts.ProblemDetails.Extensions.Add("code", _value?.Code);
        //if (_properties?.Length > 0)
        //{
        //    cts.ProblemDetails.Extensions
        //        .Add("fields", _properties.ToKeyValueList(_ => _.Code, _ => _.Message));
        //}
        // TODO: add help link here
        //    httpContext.Response.Clear();
        httpContext.Response.StatusCode = (int)defaultStatusCode;
        await httpContext.Response.WriteAsJsonAsync(problemDetails);
    }
}

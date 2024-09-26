using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace Arcturus.ResultObjects;

public sealed class StatusCodeActionResult : IActionResult
{
    private readonly Result _result;

    internal StatusCodeActionResult(Result result) => _result = result;

    public async Task ExecuteResultAsync(ActionContext context)
    {
        HttpStatusCode defaultStatusCode = _result.HttpStatusCode ?? HttpStatusCode.BadRequest;

        //var cts = new ProblemDetailsContext()
        //{
        //    HttpContext = context.HttpContext
        //};
        var factory = context.HttpContext.RequestServices.GetService<ProblemDetailsFactory>();
        var problemDetails = factory!.CreateProblemDetails(context.HttpContext, (int)defaultStatusCode);
        problemDetails.Type ??= "https://schemas/2022/fault/#" + defaultStatusCode.ToString().ToLower();
        problemDetails.Title ??= _result.Fault?.Code;
        problemDetails.Detail = _result.Fault?.Message;
        //if (_properties?.Length > 0)
        //{
        //    prbml.Extensions.Add("fields", _properties.ToKeyValueList(_ => _.Code, _ => _.Message));
        //}
        //// TODO: add helplink here
        //prbml.Extensions.Add("code", _value?.Code);
        //    context.HttpContext.Response.Clear();
        context.HttpContext.Response.StatusCode = (int)defaultStatusCode;
        await context.HttpContext.Response.WriteAsJsonAsync(problemDetails);
    }
}
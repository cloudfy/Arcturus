using Arcturus.Extensions.ResultObjects.AspNetCore.Internals;
using Arcturus.ResultObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace Arcturus.Extensions.ResultObjects.AspNetCore.ActionResults;

public sealed class ProblemDetailsActionResult : IActionResult
{
    private readonly Result _result;

    internal ProblemDetailsActionResult(Result result) => _result = result;

    public Task ExecuteResultAsync(ActionContext context)
    {
        HttpStatusCode defaultStatusCode = _result.HttpStatusCode ?? HttpStatusCode.BadRequest;

        var factory = context.HttpContext.RequestServices.GetService<ProblemDetailsFactory>();
        var problemDetails = factory!.CreateProblemDetails(context.HttpContext, (int)defaultStatusCode);
        problemDetails.Type ??= "https://schemas/2022/fault/#" + defaultStatusCode.ToString().ToLower();
        problemDetails.Title ??= _result.Fault?.Code;
        problemDetails.Detail = _result.Fault?.Message;
        problemDetails.Instance = $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";
        //if (_properties?.Length > 0)
        //{
        //    prbml.Extensions.Add("fields", _properties.ToKeyValueList(_ => _.Code, _ => _.Message));
        //}
        //// TODO: add helplink here
        //prbml.Extensions.Add("code", _value?.Code);
        //    context.HttpContext.Response.Clear();
        context.HttpContext.Response.StatusCode = (int)defaultStatusCode;
        return HttpResultsHelper.WriteResultAsJsonAsync(
            context.HttpContext
            , problemDetails);
    }
}
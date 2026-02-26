
using Arcturus.Extensions.ResultObjects.AspNetCore.Internals;
using Arcturus.ResultObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Arcturus.Extensions.ResultObjects.AspNetCore.ActionResults;

public sealed class ProblemDetailsActionResult : IActionResult
{
    private readonly Result _result;

    internal ProblemDetailsActionResult(Result result) => _result = result;

    public Task ExecuteResultAsync(ActionContext context)
    {
        var problemDetails = Internals.ProblemDetailsFactory.Create(_result, context.HttpContext);
        ProblemDetailDefaults.ApplyDefaults(problemDetails, _result, context.HttpContext);

        var problemDetailService = context.HttpContext.RequestServices.GetService<IProblemDetailsService>();
        if (problemDetailService is null)
        {
            return FallbackProblemDetailsWriter.Write(problemDetails, context.HttpContext);
        }
        else
        {
            var task = problemDetailService.WriteAsync(new ProblemDetailsContext()
            {
                HttpContext = context.HttpContext
                , ProblemDetails = problemDetails
                , Exception = _result.Exception
            });
            return task.AsTask();
        }
    }
}
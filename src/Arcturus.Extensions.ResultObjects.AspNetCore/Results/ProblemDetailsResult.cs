using Arcturus.Extensions.ResultObjects.AspNetCore.Internals;
using Arcturus.ResultObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Arcturus.Extensions.ResultObjects.AspNetCore.Results;

internal sealed class ProblemDetailsResult : IResult
{
    private readonly Result _result;

    internal ProblemDetailsResult(Result result) => _result = result;

    public Task ExecuteAsync(HttpContext httpContext)
    {
        var problemDetails = Internals.ProblemDetailsFactory.Create(_result, httpContext);
        ProblemDetailDefaults.ApplyDefaults(problemDetails, _result, httpContext);

        var problemDetailService = httpContext.RequestServices.GetService<IProblemDetailsService>();
        if (problemDetailService is null)
        {
            return FallbackProblemDetailsWriter.Write(problemDetails, httpContext);
        }
        else
        {
            var task = problemDetailService.WriteAsync(new ProblemDetailsContext()
            {
                HttpContext = httpContext
                , ProblemDetails = problemDetails
                , Exception = _result.Exception
            });
            return task.AsTask();
        }
    }
}

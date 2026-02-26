using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Arcturus.Extensions.ResultObjects.AspNetCore.Internals;

internal class FallbackProblemDetailsWriter
{
    internal static Task Write(ProblemDetails problemDetails, HttpContext httpContext)
    {
        if (httpContext.Response.HasStarted)
            throw new Exception($"Response has already started.");

        // json options are not used here because the default json options are configured by the framework
        // when writing WriteAsJsonAsync, and we want to be consistent with that.
        return httpContext.Response.WriteAsJsonAsync<ProblemDetails>(problemDetails);
    }
}
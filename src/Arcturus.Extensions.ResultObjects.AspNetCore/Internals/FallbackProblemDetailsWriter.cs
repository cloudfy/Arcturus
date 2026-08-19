//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;

//namespace Arcturus.Extensions.ResultObjects.AspNetCore.Internals;

//internal class FallbackProblemDetailsWriter
//{
//    /// <summary>
//    /// Writes the given ProblemDetails to the HttpContext response.
//    /// </summary>
//    /// <param name="problemDetails">The ProblemDetails to write to the response.</param>
//    /// <param name="httpContext">The HttpContext to write the response to.</param>
//    /// <returns>A Task representing the asynchronous operation.</returns>
//    /// <exception cref="Exception">Thrown if the response has already started.</exception>
//    internal static Task Write(ProblemDetails problemDetails, HttpContext httpContext)
//    {
//        if (httpContext.Response.HasStarted)
//            throw new Exception($"Response has already started.");

//        // json options are not used here because the default json options are configured by the framework
//        // when writing WriteAsJsonAsync, and we want to be consistent with that.
//        httpContext.Response.ContentType = "application/problem+json; charset=utf-8";
//        httpContext.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;
//        return httpContext.Response.WriteAsJsonAsync<ProblemDetails>(problemDetails);
//    }
//}
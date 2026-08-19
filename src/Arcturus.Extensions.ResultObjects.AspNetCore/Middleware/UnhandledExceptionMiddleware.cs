//using Microsoft.AspNetCore.Http;
//using Microsoft.Extensions.Logging;

//namespace Arcturus.Extensions.ResultObjects.AspNetCore.Middleware;

//public sealed class UnhandledExceptionMiddleware(
//    RequestDelegate next
//    , ILoggerFactory loggerFactory
//    //, IProblemDetailsService? problemDetailsService = null
//    , IStandardResponseFactory? standardResponseFactory = null
//    , Func<HttpContext, Exception, Task<bool>>? onExceptionEvent = null)
//{
//    //private readonly IProblemDetailsService? _problemDetailsService = problemDetailsService;
//    private readonly IStandardResponseFactory? _standardResponseFactory = standardResponseFactory;
//    private readonly ILogger<UnhandledExceptionMiddleware> _logger = loggerFactory.CreateLogger<UnhandledExceptionMiddleware>();
//    private readonly RequestDelegate _next = next;
//    private readonly Func<HttpContext, Exception, Task<bool>>? _onExceptionEvent = onExceptionEvent;

//    public async Task Invoke(HttpContext context)
//    {
//        try
//        {
//            await _next(context);
//        }
//        catch (Exception ex)
//        {
//            await HandleException(context, ex);
//        }
//    }

//    private async Task HandleException(HttpContext httpContext, Exception exception)
//    {
//        // we do not track client disconnections as unhandled exceptions, so we check for those first and log
//        // them as warnings without writing to the response body
//        if ((exception is OperationCanceledException || exception is IOException) && httpContext.RequestAborted.IsCancellationRequested)
//        {
//            _logger.LogWarning(exception, "The request was aborted by the client.");

//            if (!httpContext.Response.HasStarted)
//            {
//                httpContext.Response.StatusCode = StatusCodes.Status499ClientClosedRequest;
//            }
//            return;
//        }

//        // We can't do anything if the response has already started, just abort.
//        if (httpContext.Response.HasStarted)
//        {
//            _logger.LogWarning("The response has already started, the error details will not be written to the response body.");
//            return;
//        }

//        // call the event and if true then exit
//        if (_onExceptionEvent is not null)
//        {
//            var handled = await _onExceptionEvent.Invoke(httpContext, exception);
//            if (handled == true) return;
//        }

//        // Build a generic problem details response for unhandled exceptions.
//        // This will be passed to the IProblemDetailsService if it is registered, otherwise it will be returned as-is.
//        if (_standardResponseFactory is not null)
//        {
//            var problemDetails = _standardResponseFactory.CreateDefaultProblemDetailsResponse(httpContext);
//            await _standardResponseFactory.WriteResponse(httpContext, problemDetails, exception);
//        }
//        //// Build a generic problem details response for unhandled exceptions.
//        //// This will be passed to the IProblemDetailsService if it is registered, otherwise it will be returned as-is.
//        //var problemDetails = new ProblemDetails
//        //{
//        //    Status = 500
//        //    , Title = "An unhandled exception occurred."
//        //    , Detail = "An unhandled error occurred. Staff have been notified. Please use the traceId for reference."
//        //    , Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}"
//        //};
//        //ProblemDetailDefaults.ApplyDefaults(problemDetails, null, httpContext, exception);

//        //if (_problemDetailsService is not null)
//        //{
//        //    await _problemDetailsService.WriteAsync(new ProblemDetailsContext
//        //    {
//        //        HttpContext = httpContext
//        //        , Exception = exception
//        //        , ProblemDetails = problemDetails
//        //    });
//        //}
//        //else
//        //{
//        //    await FallbackProblemDetailsWriter.Write(problemDetails, httpContext);
//        //}
//    }
//}
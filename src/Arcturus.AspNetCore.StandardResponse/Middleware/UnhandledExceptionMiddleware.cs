using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Arcturus.AspNetCore.StandardResponse.Middleware;

public sealed class UnhandledExceptionMiddleware(
    RequestDelegate next
    , ILoggerFactory loggerFactory
    , IStandardResponseFactory standardResponseFactory
    , Func<HttpContext, Exception, Task<bool>>? onExceptionEvent = null)
{
    private readonly IStandardResponseFactory _standardResponseFactory = standardResponseFactory;
    private readonly ILogger<UnhandledExceptionMiddleware> _logger = loggerFactory.CreateLogger<UnhandledExceptionMiddleware>();
    private readonly RequestDelegate _next = next;
    private readonly Func<HttpContext, Exception, Task<bool>>? _onExceptionEvent = onExceptionEvent;

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (BadHttpRequestException ex) 
        {
            await HandleBadRequest(context, ex);
        }
        catch (Exception ex)
        {
            await HandleException(context, ex);
        }
    }

    private async Task HandleBadRequest(HttpContext httpContext, BadHttpRequestException exception)
    {
        // We can't do anything if the response has already started, just abort.
        if (httpContext.Response.HasStarted)
        {
            _logger.LogWarning("The response has already started, the error details will not be written to the response body.");
            return;
        }

        // call the event and if true then exit
        if (_onExceptionEvent is not null)
        {
            var handled = await _onExceptionEvent.Invoke(httpContext, exception);
            if (handled == true) return;
        }

        // Build a generic problem details response for unhandled exceptions.
        var badRequest = _standardResponseFactory.CreateProblemDetailsResponse(
            "Bad request"
            , null
            , null
            , System.Net.HttpStatusCode.BadRequest);
        await _standardResponseFactory.WriteResponse(httpContext, badRequest, exception);
    }

    private async Task HandleException(HttpContext httpContext, Exception exception)
    {
        // we do not track client disconnections as unhandled exceptions, so we check for those first and log
        // them as warnings without writing to the response body
        if ((exception is OperationCanceledException || exception is IOException) && httpContext.RequestAborted.IsCancellationRequested)
        {
            _logger.LogWarning(exception, "The request was aborted by the client.");

            if (!httpContext.Response.HasStarted)
            {
                httpContext.Response.StatusCode = StatusCodes.Status499ClientClosedRequest;
            }
            return;
        }

        // We can't do anything if the response has already started, just abort.
        if (httpContext.Response.HasStarted)
        {
            _logger.LogWarning("The response has already started, the error details will not be written to the response body.");
            return;
        }

        // call the event and if true then exit
        if (_onExceptionEvent is not null)
        {
            var handled = await _onExceptionEvent.Invoke(httpContext, exception);
            if (handled == true) return;
        }

        // Build a generic problem details response for unhandled exceptions.
        // This will be passed to the IProblemDetailsService if it is registered, otherwise it will be returned as-is.
        var problemDetails = _standardResponseFactory.CreateDefaultProblemDetailsResponse(httpContext);
        await _standardResponseFactory.WriteResponse(httpContext, problemDetails, exception);
    }
}

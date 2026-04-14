using Arcturus.Extensions.ResultObjects.AspNetCore.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Arcturus.Extensions.ResultObjects.AspNetCore;

public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Adds Arcturus UnhandledException middleware to the application's request pipeline that handles unhandled exceptions and provides a
    /// consistent error response (HTTP 500).
    /// </summary>
    /// <remarks>This method registers the UnhandledExceptionMiddleware, which intercepts unhandled exceptions
    /// during request processing and ensures that clients receive a standardized error response. Use this method early
    /// in the pipeline to catch exceptions from subsequent middleware.</remarks>
    /// <param name="app">The application builder used to configure the middleware for handling unhandled exceptions. Cannot be null.</param>
    /// <param name="onExceptionEvent">An optional callback that is invoked when an unhandled exception occurs. Can be used for custom logging or other actions. If returning true, the handler stops executing.</param>
    /// <returns>The same instance of the <see cref="IApplicationBuilder" />, enabling method chaining.</returns>
    public static IApplicationBuilder UseUnhandledExceptionHandler(
        this IApplicationBuilder app
        , Func<HttpContext, Exception, bool>? onExceptionEvent = null)
    {
        if (onExceptionEvent is not null)
            return app.UseMiddleware<UnhandledExceptionMiddleware>(onExceptionEvent);
        return app.UseMiddleware<UnhandledExceptionMiddleware>();
    }
}

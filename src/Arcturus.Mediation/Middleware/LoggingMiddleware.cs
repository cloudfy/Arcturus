using Arcturus.Mediation.Abstracts;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Arcturus.Mediation.Middleware;

/// <summary>
/// Middleware that logs request handling information.
/// </summary>
public class LoggingMiddleware : IMiddleware
{
    private readonly ILogger<LoggingMiddleware> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoggingMiddleware"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    public LoggingMiddleware(ILogger<LoggingMiddleware> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task InvokeAsync(IMiddlewareContext context, PipelineRequestDelegate next)
    {
        var requestType = context.RequestType;
        var correlationId = Guid.NewGuid().ToString("N")[..8];

        context.Items["CorrelationId"] = correlationId;

        _logger.LogInformation("Handling request {RequestType} [{CorrelationId}]",
            requestType.Name, correlationId);

        var stopwatch = Stopwatch.StartNew();

        try
        {
            await next();
            stopwatch.Stop();

            _logger.LogInformation("Successfully handled request {RequestType} [{CorrelationId}] in {ElapsedMs}ms",
                requestType.Name, correlationId, stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            _logger.LogError(ex, "Failed to handle request {RequestType} [{CorrelationId}] after {ElapsedMs}ms",
                requestType.Name, correlationId, stopwatch.ElapsedMilliseconds);

            throw;
        }
    }
}

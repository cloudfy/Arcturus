using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Arcturus.Extensions.ResultObjects.AspNetCore;

public sealed class ArcturusAspNetCoreProblemDetailsFactory(
    IOptions<ApiBehaviorOptions> options,
    IHostEnvironment environment) : ProblemDetailsFactory
{
    private readonly ApiBehaviorOptions _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
    private readonly IHostEnvironment _environment = environment ?? throw new ArgumentNullException(nameof(environment));

    public override ProblemDetails CreateProblemDetails(
        HttpContext httpContext,
        int? statusCode = null,
        string? title = null,
        string? type = null,
        string? detail = null,
        string? instance = null)
    {
        statusCode ??= 500;

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Type = type,
            Detail = detail,
            Instance = instance
        };

        ApplyDefaults(httpContext, problemDetails, statusCode.Value);
        return problemDetails;
    }

    public override ValidationProblemDetails CreateValidationProblemDetails(
        HttpContext httpContext,
        ModelStateDictionary modelStateDictionary,
        int? statusCode = null,
        string? title = null,
        string? type = null,
        string? detail = null,
        string? instance = null)
    {
        if (modelStateDictionary == null)
        {
            throw new ArgumentNullException(nameof(modelStateDictionary));
        }

        statusCode ??= 400;

        var problemDetails = new ValidationProblemDetails(modelStateDictionary)
        {
            Status = statusCode,
            Type = type,
            Detail = detail,
            Instance = instance,
            Title = title
        };

        ApplyDefaults(httpContext, problemDetails, statusCode.Value);
        return problemDetails;
    }

    private void ApplyDefaults(HttpContext httpContext, ProblemDetails problemDetails, int statusCode)
    {
        problemDetails.Status ??= statusCode;

        //var x = new ApiBehaviorOptions().ClientErrorMapping.
        // Add default title and type from RFC if not provided
        if (_options.ClientErrorMapping.TryGetValue(statusCode, out var clientErrorData))
        {
            problemDetails.Title ??= clientErrorData.Title;
            problemDetails.Type ??= clientErrorData.Link;
        }
        else
        {
            problemDetails.Type ??= $"https://httpstatuses.io/{statusCode}";
            problemDetails.Title ??= $"HTTP {statusCode}";
        }

        // Instance URI
        problemDetails.Instance ??= httpContext.Request.Path;

        // Add trace ID or correlation ID
        string traceId = httpContext.TraceIdentifier;
        if (httpContext.Request.Headers.TryGetValue("X-Correlation-ID", out var correlationId))
        {
            problemDetails.Extensions["correlationId"] = correlationId.ToString();
        }
        else if (!string.IsNullOrEmpty(traceId))
        {
            problemDetails.Extensions["correlationId"] = traceId;
        }
        if (traceId != null)
        {
            problemDetails.Extensions["traceId"] = traceId;
        }

        // Include detailed errors only in Development
        if (_environment.IsDevelopment())
        {
            if (httpContext.Features.Get<IExceptionHandlerFeature>()?.Error is Exception ex)
            {
                problemDetails.Extensions["exception"] = new
                {
                    message = ex.Message,
                    type = ex.GetType().FullName,
                    stackTrace = ex.StackTrace?.Split(Environment.NewLine)
                };
            }
        }
    }
}

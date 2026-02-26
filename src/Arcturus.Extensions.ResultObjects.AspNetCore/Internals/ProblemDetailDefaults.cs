using Arcturus.ResultObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Net;
using System.Text.Json;

namespace Arcturus.Extensions.ResultObjects.AspNetCore.Internals;

internal static class ProblemDetailDefaults
{
    private static JsonNamingPolicy? _cachedNamingPolicy { get; set; }

    internal const HttpStatusCode DefaultStatusCode = HttpStatusCode.BadRequest;

    internal static void ApplyDefaults(ProblemDetails problemDetails, Result? result, HttpContext httpContext)
    {
        var namingPolicy = _cachedNamingPolicy ?? ResolveNamingPolicy(httpContext);

        var correlationIdKey = namingPolicy != null ? namingPolicy.ConvertName("CorrelationId") : "correlationId";
        var traceIdKey = namingPolicy != null ? namingPolicy.ConvertName("TraceId") : "traceId";
        var timestampKey = namingPolicy != null ? namingPolicy.ConvertName("Timestamp") : "timestamp";

        // Add trace ID or correlation ID
        string traceId = httpContext.TraceIdentifier;
        if (!problemDetails.Extensions.ContainsKey(correlationIdKey) &&
            httpContext.Request.Headers.TryGetValue("X-Correlation-ID", out var correlationId))
        {
            problemDetails.Extensions[correlationIdKey] = correlationId.ToString();
        }
        else if (
            !problemDetails.Extensions.ContainsKey(correlationIdKey) &&
            !string.IsNullOrEmpty(traceId))
        {
            problemDetails.Extensions[correlationIdKey] = traceId;
        }
        if (!problemDetails.Extensions.ContainsKey(traceIdKey) &&
            traceId != null)
        {
            problemDetails.Extensions[traceIdKey] = traceId;
        }
        problemDetails.Extensions[timestampKey] = DateTime.UtcNow;
        problemDetails.Instance ??= $"{httpContext.Request.Method} {httpContext.Request.Path}";
        problemDetails.Type ??= $"https://schemas/2022/fault/#{result?.HttpStatusCode?.ToString() ?? "400"}";
        problemDetails.Title ??= result?.Fault?.Code;
        problemDetails.Detail = result?.Fault?.Message;        
    }

    private static JsonNamingPolicy? ResolveNamingPolicy(HttpContext httpContext)
    {
        // Try to get JsonOptions from Minimal API configuration using IOptions
        var jsonOptions = httpContext.RequestServices
            .GetService<IOptions<Microsoft.AspNetCore.Http.Json.JsonOptions>>()?.Value;
        if (jsonOptions?.SerializerOptions is not null)
        {
            var serializerOptions = jsonOptions.SerializerOptions;
            
            // Check if there's a TypeInfoResolver and get ProblemDetails-specific configuration
            if (serializerOptions.TypeInfoResolver != null)
            {
                var typeInfo = serializerOptions.TypeInfoResolver.GetTypeInfo(
                    typeof(ProblemDetails), 
                    serializerOptions);
                
                // Check if the type info has a specific naming policy configured
                if (typeInfo?.Options?.PropertyNamingPolicy != null)
                {
                    return typeInfo.Options.PropertyNamingPolicy;
                }
            }
            
            // Fall back to global PropertyNamingPolicy if no type-specific policy
            if (serializerOptions.PropertyNamingPolicy != null)
            {
                return serializerOptions.PropertyNamingPolicy;
            }
        }

        // Fallback to MVC JsonOptions using IOptions
        var mvcJsonOptions = httpContext.RequestServices
            .GetService<IOptions<Microsoft.AspNetCore.Mvc.JsonOptions>>()?.Value;
        if (mvcJsonOptions?.JsonSerializerOptions is not null)
        {
            var serializerOptions = mvcJsonOptions.JsonSerializerOptions;
            
            // Check MVC's TypeInfoResolver for ProblemDetails-specific configuration
            if (serializerOptions.TypeInfoResolver != null)
            {
                var typeInfo = serializerOptions.TypeInfoResolver.GetTypeInfo(
                    typeof(ProblemDetails), 
                    serializerOptions);
                
                if (typeInfo?.Options?.PropertyNamingPolicy != null)
                {
                    return typeInfo.Options.PropertyNamingPolicy;
                }
            }
            
            if (serializerOptions.PropertyNamingPolicy != null)
            {
                return serializerOptions.PropertyNamingPolicy;
            }
        }

        // Default to camelCase if nothing is configured
        return JsonNamingPolicy.CamelCase;
    }

    private static readonly Dictionary<int, (string Type, string Title)> Defaults = new()
    {
        [400] =
        (
            "https://tools.ietf.org/html/rfc9110#section-15.5.1",
            "Bad Request"
        ),

        [401] =
        (
            "https://tools.ietf.org/html/rfc9110#section-15.5.2",
            "Unauthorized"
        ),

        [403] =
        (
            "https://tools.ietf.org/html/rfc9110#section-15.5.4",
            "Forbidden"
        ),

        [404] =
        (
            "https://tools.ietf.org/html/rfc9110#section-15.5.5",
            "Not Found"
        ),

        [405] =
        (
            "https://tools.ietf.org/html/rfc9110#section-15.5.6",
            "Method Not Allowed"
        ),

        [406] =
        (
            "https://tools.ietf.org/html/rfc9110#section-15.5.7",
            "Not Acceptable"
        ),

        [407] =
        (
            "https://tools.ietf.org/html/rfc9110#section-15.5.8",
            "Proxy Authentication Required"
        ),

        [408] =
        (
            "https://tools.ietf.org/html/rfc9110#section-15.5.9",
            "Request Timeout"
        ),

        [409] =
        (
            "https://tools.ietf.org/html/rfc9110#section-15.5.10",
            "Conflict"
        ),

        [410] =
        (
            "https://tools.ietf.org/html/rfc9110#section-15.5.11",
            "Gone"
        ),

        [411] =
        (
            "https://tools.ietf.org/html/rfc9110#section-15.5.12",
            "Length Required"
        ),

        [412] =
        (
            "https://tools.ietf.org/html/rfc9110#section-15.5.13",
            "Precondition Failed"
        ),

        [413] =
        (
            "https://tools.ietf.org/html/rfc9110#section-15.5.14",
            "Content Too Large"
        ),

        [414] =
        (
            "https://tools.ietf.org/html/rfc9110#section-15.5.15",
            "URI Too Long"
        ),

        [415] =
        (
            "https://tools.ietf.org/html/rfc9110#section-15.5.16",
            "Unsupported Media Type"
        ),

        [416] =
        (
            "https://tools.ietf.org/html/rfc9110#section-15.5.17",
            "Range Not Satisfiable"
        ),

        [417] =
        (
            "https://tools.ietf.org/html/rfc9110#section-15.5.18",
            "Expectation Failed"
        ),

        [421] =
        (
            "https://tools.ietf.org/html/rfc9110#section-15.5.20",
            "Misdirected Request"
        ),

        [422] =
        (
            "https://tools.ietf.org/html/rfc4918#section-11.2",
            "Unprocessable Entity"
        ),

        [426] =
        (
            "https://tools.ietf.org/html/rfc9110#section-15.5.22",
            "Upgrade Required"
        ),

        [500] =
        (
            "https://tools.ietf.org/html/rfc9110#section-15.6.1",
            "An error occurred while processing your request."
        ),

        [501] =
        (
            "https://tools.ietf.org/html/rfc9110#section-15.6.2",
            "Not Implemented"
        ),

        [502] =
        (
            "https://tools.ietf.org/html/rfc9110#section-15.6.3",
            "Bad Gateway"
        ),

        [503] =
        (
            "https://tools.ietf.org/html/rfc9110#section-15.6.4",
            "Service Unavailable"
        ),

        [504] =
        (
            "https://tools.ietf.org/html/rfc9110#section-15.6.5",
            "Gateway Timeout"
        ),

        [505] =
        (
            "https://tools.ietf.org/html/rfc9110#section-15.6.6",
            "HTTP Version Not Supported"
        ),
    };
}
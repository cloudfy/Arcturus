using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json.Serialization.Metadata;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc;

namespace Arcturus.Extensions.ResultObjects.AspNetCore.Internals;

internal static class HttpResultsHelper
{
    private static JsonOptions ResolveJsonOptions(HttpContext httpContext)
    {
        // Attempt to resolve options from DI then fallback to default options
        return httpContext.RequestServices.GetService<IOptions<JsonOptions>>()?.Value ?? new JsonOptions();
    }

    public static Task WriteResultAsJsonAsync<TValue>(
        HttpContext httpContext,
        TValue? value,
        string? contentType = null,
        JsonSerializerOptions? jsonSerializerOptions = null)
    {
        if (value is null)
        {
            return Task.CompletedTask;
        }

        jsonSerializerOptions ??= ResolveJsonOptions(httpContext).JsonSerializerOptions;
        var jsonTypeInfo = (JsonTypeInfo<TValue>)jsonSerializerOptions.GetTypeInfo(typeof(TValue));

        Type? runtimeType = value.GetType();
        if (jsonTypeInfo.ShouldUseWith(runtimeType))
        {
            return httpContext.Response.WriteAsJsonAsync(
                value,
                jsonTypeInfo,
                contentType: contentType);
        }

        // Since we don't know the type's polymorphic characteristics
        // our best option is to serialize the value as 'object'.
        // call WriteAsJsonAsync<object>() rather than the declared type
        // and avoid source generators issues.
        // https://github.com/dotnet/aspnetcore/issues/43894
        // https://learn.microsoft.com/dotnet/standard/serialization/system-text-json-polymorphism
        return httpContext.Response.WriteAsJsonAsync<object>(
           value,
           jsonSerializerOptions,
           contentType: contentType);
    }
}

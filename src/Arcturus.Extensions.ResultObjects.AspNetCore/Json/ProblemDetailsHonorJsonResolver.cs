using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace Arcturus.Extensions.AspNetCore.Json.Serialization;

internal sealed class ProblemDetailsHonorJsonResolver(
    IJsonTypeInfoResolver inner) 
    : IJsonTypeInfoResolver
{
    private readonly IJsonTypeInfoResolver _inner = inner ?? throw new ArgumentNullException(nameof(inner));

    public JsonTypeInfo? GetTypeInfo(Type type, JsonSerializerOptions options)
    {
        var ti = _inner.GetTypeInfo(type, options);
        if (ti is null)
            return null;

        // Apply only to ProblemDetails and derived (e.g., ValidationProblemDetails)
        if (!typeof(ProblemDetails).IsAssignableFrom(ti.Type))
            return ti;

        // Only object contracts have Properties
        if (ti.Kind != JsonTypeInfoKind.Object)
            return ti;

        // Use the naming policy from JsonSerializerOptions
        var namingPolicy = options.PropertyNamingPolicy;
        if (namingPolicy is not null)
        {
            foreach (var prop in ti.Properties)
            {
                // Apply the naming policy to the original property name
                prop.Name = namingPolicy.ConvertName(prop.Name);
            }
        }

        return ti;
    }
}
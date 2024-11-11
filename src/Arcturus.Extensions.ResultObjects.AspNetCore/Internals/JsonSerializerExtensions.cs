using System.Text.Json.Serialization.Metadata;
using System.Text.Json;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Arcturus.Extensions.ResultObjects.AspNetCore.Internals;

internal static class JsonSerializerExtensions
{
    internal static bool HasKnownPolymorphism(this JsonTypeInfo jsonTypeInfo)
     => jsonTypeInfo.Type.IsSealed || jsonTypeInfo.Type.IsValueType || jsonTypeInfo.PolymorphismOptions is not null;

    internal static bool ShouldUseWith(this JsonTypeInfo jsonTypeInfo, [NotNullWhen(false)] Type? runtimeType)
     => runtimeType is null || jsonTypeInfo.Type == runtimeType || jsonTypeInfo.HasKnownPolymorphism();

    internal static JsonTypeInfo GetReadOnlyTypeInfo(this JsonSerializerOptions options, Type type)
    {
        options.MakeReadOnly();
        return options.GetTypeInfo(type);
    }

    internal static JsonTypeInfo GetRequiredTypeInfo(this JsonSerializerContext context, Type type)
        => context.GetTypeInfo(type) ?? throw new InvalidOperationException($"Unable to obtain the JsonTypeInfo for type '{type.FullName}' from the context '{context.GetType().FullName}'.");
}

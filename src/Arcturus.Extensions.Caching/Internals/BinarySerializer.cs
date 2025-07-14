using System.Text.Json;

namespace Arcturus.Extensions.Caching.Internals;

internal static class BinarySerializer
{
    private static readonly JsonSerializerOptions _options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
    };

    internal static byte[]? Serialize(object? obj)
    {
        if (obj is null)
            return null;

        return JsonSerializer.SerializeToUtf8Bytes(obj, _options);
    }

    internal static T? Deserialize<T>(byte[]? data) where T : class
    {
        return JsonSerializer.Deserialize<T>(data, _options);
    }
}
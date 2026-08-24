using System.Text.Json;

namespace Arcturus.Repository.Json.Internals;

/// <summary>
/// Provides a static class for configuring and retrieving JSON serialization options for Entity Framework.
/// </summary>
internal static class SpecificEfJsonSerializer
{
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };
    internal static void ConfigureJsonOptions(Action<JsonSerializerOptions> options)
    {
        options(_jsonOptions);
    }

    internal static JsonSerializerOptions GetJsonOptions()
    {
        return _jsonOptions;
    }
}

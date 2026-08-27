using System.Text.Json;
using System.Text.Json.Serialization;

namespace Arcturus.SmartEnums;

public sealed class SmartEnumJsonConverter<T> : JsonConverter<T>
    where T : ISmartEnum<T>
{
    public override T Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        var value = reader.GetString()
            ?? throw new JsonException("Expected a string.");

        try
        {
            return T.FromValue(value);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            throw new JsonException(
                $"Unknown {typeof(T).Name} value '{value}'.",
                ex);
        }
    }

    public override void Write(
        Utf8JsonWriter writer,
        T value,
        JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Value);
    }
}
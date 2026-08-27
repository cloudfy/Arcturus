using System.Text.Json;
using System.Text.Json.Serialization;

namespace Arcturus.SmartEnums;

/// <summary>
/// JSON converter for smart enum types that implement <see cref="ISmartEnum{T}"/>.
/// Serializes smart enums as their underlying string values and deserializes strings back to smart enum instances.
/// </summary>
/// <typeparam name="T">The smart enum type to convert.</typeparam>
public sealed class SmartEnumJsonConverter<T> : JsonConverter<T>
    where T : ISmartEnum<T>
{
    /// <summary>
    /// Reads a JSON string value and converts it to a smart enum instance.
    /// </summary>
    /// <param name="reader">The reader to read from.</param>
    /// <param name="typeToConvert">The type to convert.</param>
    /// <param name="options">The serializer options.</param>
    /// <returns>The smart enum instance corresponding to the JSON value.</returns>
    /// <exception cref="JsonException">Thrown when the JSON value is not a string or not a recognized enum value.</exception>
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

    /// <summary>
    /// Writes a smart enum instance as its underlying string value.
    /// </summary>
    /// <param name="writer">The writer to write to.</param>
    /// <param name="value">The smart enum value to write.</param>
    /// <param name="options">The serializer options.</param>
    public override void Write(
        Utf8JsonWriter writer,
        T value,
        JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Value);
    }
}
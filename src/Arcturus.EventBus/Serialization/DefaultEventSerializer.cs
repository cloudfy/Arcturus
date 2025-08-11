using Arcturus.EventBus.Abstracts;
using System.Text.Json;

namespace Arcturus.EventBus.Serialization;

/// <summary>
/// Provides static methods for serializing and deserializing <see cref="IEventMessage"/> instances
/// using System.Text.Json with custom options and converters.
/// </summary>
public static class DefaultEventSerializer
{
    private static readonly JsonSerializerOptions _options = new()
    {
        WriteIndented = false
        , DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        , Converters = { new DefaultEventMessageConverter() }
    };


    /// <summary>
    /// Serializes the specified <see cref="IEventMessage"/> instance to a JSON string using custom options and converters.
    /// </summary>
    /// <param name="event">The event message to serialize. Must not be null.</param>
    /// <returns>A JSON string representation of the event message.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="event"/> is null.</exception>
    public static string Serialize(IEventMessage @event)
    {
        if (@event == null)
            throw new ArgumentNullException(nameof(@event), "Event cannot be null.");
        return JsonSerializer.Serialize(@event, _options);
    }
    /// <summary>
    /// Deserializes the specified JSON string to an <see cref="IEventMessage"/> instance using custom options and converters.
    /// </summary>
    /// <param name="data">The JSON string to deserialize. Must not be null or empty.</param>
    /// <returns>The deserialized <see cref="IEventMessage"/> instance.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="data"/> is null or empty.</exception>
    /// <exception cref="InvalidOperationException">Thrown if deserialization returns null.</exception>
    public static IEventMessage Deserialize(string data)
    {
        if (string.IsNullOrEmpty(data))
            throw new ArgumentNullException(nameof(data), "Data cannot be null or empty.");
        return JsonSerializer.Deserialize<IEventMessage>(data, _options) ?? throw new InvalidOperationException("Deserialization returned null.");
    }
}

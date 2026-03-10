using Arcturus.EventBus.Abstracts;

namespace Arcturus.EventBus.Serialization;

public interface IDefaultEventMessageSerializer
{
    /// <summary>
    /// Deserializes the specified JSON string to an <see cref="IEventMessage"/> instance using custom options and converters.
    /// </summary>
    /// <param name="data">The JSON string to deserialize. Must not be null or empty.</param>
    /// <returns>The deserialized <see cref="IEventMessage"/> instance.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="data"/> is null or empty.</exception>
    /// <exception cref="InvalidOperationException">Thrown if deserialization returns null.</exception>
    IEventMessage Deserialize(string data);

    /// <summary>
    /// Serializes the specified <see cref="IEventMessage"/> instance to a JSON string using custom options and converters.
    /// </summary>
    /// <param name="event">The event message to serialize. Must not be null.</param>
    /// <returns>A JSON string representation of the event message.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="event"/> is null.</exception>

    string Serialize(IEventMessage @event);
}
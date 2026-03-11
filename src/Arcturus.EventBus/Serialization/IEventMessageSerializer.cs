using Arcturus.EventBus.Abstracts;

namespace Arcturus.EventBus.Serialization;

/// <summary>
/// Defines methods for serializing and deserializing event messages to and from string representations.
/// </summary>
/// <remarks>Implementations of this interface are responsible for converting event messages into a format
/// suitable for transmission or storage, and for reconstructing event messages from such formats. The serialization and
/// deserialization processes must handle the expected data formats correctly to ensure interoperability and data
/// integrity.</remarks>
public interface IEventMessageSerializer
{
    /// <summary>
    /// Deserializes the specified string into an event message instance.
    /// </summary>
    /// <remarks>If the input string is not in a valid format, deserialization may fail and an
    /// exception may be thrown. Ensure that the input data matches the expected serialization format.</remarks>
    /// <param name="data">The string representation of the event message to deserialize. The format must be compatible with the
    /// serializer implementation.</param>
    /// <returns>An instance of <see cref="IEventMessage"/> that represents the deserialized event message.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="UnprocessableEventException"></exception>
    /// <exception cref="System.Text.Json.JsonException"></exception>
    IEventMessage Deserialize(string data);
    /// <summary>
    /// Serializes the specified event message to a string representation suitable for transmission or storage.
    /// </summary>
    /// <remarks>Ensure that the event message contains all required data before serialization.
    /// Missing or incomplete information in the event message may result in an incomplete or invalid serialized
    /// output.</remarks>
    /// <param name="event">The event message to serialize. This parameter cannot be null and must implement the IEventMessage
    /// interface.</param>
    /// <returns>A string that represents the serialized form of the event message.</returns>
    string Serialize(IEventMessage @event);
}
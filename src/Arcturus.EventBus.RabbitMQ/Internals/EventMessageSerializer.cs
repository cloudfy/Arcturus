using Arcturus.EventBus.Abstracts;
using System.Text.Json;

namespace Arcturus.EventBus.RabbitMQ.Internals;

internal static class EventMessageSerializer
{
    private static JsonSerializerOptions _options = new()
    {
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        , WriteIndented = false
        , Converters = { new EventMessageConverter() }
    };

    internal static string Serialize(IEventMessage @event)
        => JsonSerializer.Serialize<IEventMessage>(@event, _options);
    internal static IEventMessage Deserialize(string message)
        => JsonSerializer.Deserialize<IEventMessage>(message, _options)!;
}

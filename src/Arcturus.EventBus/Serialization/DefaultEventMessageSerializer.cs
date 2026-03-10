using Arcturus.EventBus.Abstracts;
using System.Text.Json;

namespace Arcturus.EventBus.Serialization;

/// <summary>
/// Provides static methods for serializing and deserializing <see cref="IEventMessage"/> instances
/// using System.Text.Json with custom options and converters.
/// </summary>
public class DefaultEventMessageSerializer : IDefaultEventMessageSerializer
{
    private readonly JsonSerializerOptions _options;
    
    public DefaultEventMessageSerializer(Assembly[] handlerAssemblies)
    {
        _options = new()
        {
            WriteIndented = false
            , DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            , Converters = { new DefaultEventMessageConverter(handlerAssemblies) }
        };
    }
    
    public string Serialize(IEventMessage @event)
    {
        if (@event == null)
            throw new ArgumentNullException(nameof(@event), "Event cannot be null.");
        return JsonSerializer.Serialize(@event, _options);
    }
    public IEventMessage Deserialize(string data)
    {
        if (string.IsNullOrEmpty(data))
            throw new ArgumentNullException(nameof(data), "Data cannot be null or empty.");
        return JsonSerializer.Deserialize<IEventMessage>(data, _options) ?? throw new InvalidOperationException("Deserialization returned null.");
    }
}

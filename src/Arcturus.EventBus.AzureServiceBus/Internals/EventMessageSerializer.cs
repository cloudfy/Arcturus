//using Arcturus.EventBus.Abstracts;
//using Arcturus.EventBus.Serialization;
//using System.Text.Json;

//namespace Arcturus.EventBus.AzureServiceBus.Internals;

//public class EventMessageSerializer(DefaultEventMessageTypeResolver defaultEventMessageTypeResolver)
//    : IEventMessageSerializer
//{
//    private readonly JsonSerializerOptions _options = new()
//    {
//        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
//        , WriteIndented = false
//        , Converters = { new Serialization.DefaultEventMessageConverter(defaultEventMessageTypeResolver) }
//    };

//    public string Serialize(IEventMessage @event)
//        => JsonSerializer.Serialize<IEventMessage>(@event, _options);
//    public IEventMessage Deserialize(string message)
//        => JsonSerializer.Deserialize<IEventMessage>(message, _options)!;
//}
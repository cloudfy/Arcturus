//using Arcturus.EventBus.Abstracts;
//using System.Text.Json;

//namespace Arcturus.EventBus.Sqlite.Internals;

//internal static class EventSerializer
//{
//    private static readonly JsonSerializerOptions _options = new ()
//    {
//        WriteIndented = false
//        , DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
//        , Converters = { new Serialization.DefaultEventMessageConverter() }
//    };

//    internal static string Serialize(IEventMessage @event)
//    {
//        if (@event == null)
//            throw new ArgumentNullException(nameof(@event), "Event cannot be null.");
//        return JsonSerializer.Serialize(@event, _options);
//    }
//    internal static IEventMessage Deserialize(string data)
//    {
//        if (string.IsNullOrEmpty(data))
//            throw new ArgumentNullException(nameof(data), "Data cannot be null or empty.");
//        return JsonSerializer.Deserialize<IEventMessage>(data, _options) ?? throw new InvalidOperationException("Deserialization returned null.");
//    }
//}

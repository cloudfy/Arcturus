using Arcturus.EventBus.Abstracts;

namespace Arcturus.EventBus.Serialization
{
    public interface IEventMessageSerializer
    {
        IEventMessage Deserialize(string data);
        string Serialize(IEventMessage @event);
    }
}
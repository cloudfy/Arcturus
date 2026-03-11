using System.Reflection;

namespace Arcturus.EventBus.Abstracts;

public static class EventMessageExtensions
{
    /// <summary>
    /// Gets the name of the event, either from the <see cref="EventMessageAttribute"/> or the class fullname (including namespace).
    /// </summary>
    /// <param name="event">Event.</param>
    /// <returns>Name of event.</returns>
    public static string GetEventName(this IEventMessage @event)
    {
        var messageAttribute = @event.GetType().GetCustomAttribute<EventMessageAttribute>();
        if (messageAttribute is not null)
            return messageAttribute.Name;

        return @event.GetType().FullName!;
    }
}

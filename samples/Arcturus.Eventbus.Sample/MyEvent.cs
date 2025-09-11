// Create and configure the host
using Arcturus.EventBus.Abstracts;

public class MyEvent : IEventMessage
{
    public string Name { get; set; } = "My Event";
}
public class MyEventHandler : IEventMessageHandler<MyEvent>
{
    public Task Handle(MyEvent @event, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"Handling event: {@event.Name}");

        return Task.CompletedTask;
    }
}
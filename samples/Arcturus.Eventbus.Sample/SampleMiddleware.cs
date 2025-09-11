using Arcturus.EventBus.Middleware;

namespace Arcturus.Eventbus.Sample;

public class SampleMiddleware(EventRequestDelegate next)
{
    private readonly EventRequestDelegate _next = next;

    public async Task InvokeAsync(EventContext context)
    {
        Console.WriteLine($"SampleMiddleware: Processing event {context.EventName}...");
    }
}

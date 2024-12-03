namespace Arcturus.EventBus.Middleware;

public interface IEventMiddleware
{
    Task InvokeAsync(EventContext context);
}
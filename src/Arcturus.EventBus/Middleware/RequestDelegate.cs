namespace Arcturus.EventBus.Middleware;

/// <summary>
/// Represents a method that can process an event in the event bus middleware.
/// </summary>
/// <param name="context">The context of the event being processed.</param>
/// <returns>A <see cref="Task" /> that represents the completion of the event processing.</returns>
public delegate Task RequestDelegate(EventContext context);

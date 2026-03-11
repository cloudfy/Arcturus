using Arcturus.EventBus.Abstracts;
using Arcturus.EventBus.Internals;
using Arcturus.EventBus.Middleware;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Arcturus.EventBus;

/// <summary>
/// Initializes a new instance of the auto-event wired message processor. This handles the events.
/// </summary>
public sealed class EventHandlersProcessor : IProcessor
{
    private readonly IProcessor _processor;
    private readonly EventTypeRegistry _eventTypeRegistry;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<EventHandlersProcessor> _logger;

    public EventHandlersProcessor(
        IProcessor processor, IServiceProvider serviceProvider, ILoggerFactory loggerFactory)
    {
        _processor = processor;
        _processor.OnProcessAsync += InternalOnProcessAsync;
        _eventTypeRegistry = serviceProvider.GetRequiredService<EventTypeRegistry>();
        _serviceProvider = serviceProvider;

        _logger = loggerFactory.CreateLogger<EventHandlersProcessor>();
    }

    /// <inheritdoc />
    public event Func<IEventMessage, OnProcessEventArgs?, Task>? OnProcessAsync;

    /// <inheritdoc />
    public async Task WaitForEvents(CancellationToken cancellationToken = default)
        => await _processor.WaitForEvents(cancellationToken);

    private async Task InternalOnProcessAsync(IEventMessage @event, OnProcessEventArgs? e)
    {
        // O(1) dictionary lookup — MakeGenericType, GetMethod, and IsAssignableFrom are all pre-computed at startup.
        var handlerEntry = _eventTypeRegistry.GetHandlerEntryByEventType(@event);

        if (handlerEntry is null)
        {
            _logger.LogTrace("There are no handlers for the following event: {EventName}. Attempting OnProcessAsync delegate.", @event.GetType().Name);

            if (OnProcessAsync is not null)
                await OnProcessAsync!.Invoke(@event, e);

            return;
        }

        await using var scope = _serviceProvider.CreateAsyncScope();
        try
        {
            var handler = ActivatorUtilities.GetServiceOrCreateInstance(scope.ServiceProvider, handlerEntry.HandlerType);
            if (handler == null)
            {
                _logger.LogTrace("There are no handlers for the following event: {EventName}", @event.GetType().Name);
                return;
            }

            var pipeline = HostExtensions.BuildByRequestDelegate(
                async () =>
                {
                    await (Task)handlerEntry.HandleMethod.Invoke(handler, [@event!, e?.CancellationToken])!;
                });

            await pipeline(new EventContext(@event, nameof(@event), scope.ServiceProvider));
        }
        catch
        {
            throw;
        }
    }
}
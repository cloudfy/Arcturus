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
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<EventHandlersProcessor> _logger;

    public EventHandlersProcessor(
        IProcessor processor, IServiceProvider serviceProvider, ILoggerFactory loggerFactory)
    {
        _processor = processor;
        _processor.OnProcessAsync += InternalOnProcessAsync;

        _serviceProvider = serviceProvider;

        _logger = loggerFactory.CreateLogger<EventHandlersProcessor>();
    }
    /// <inheritdoc />
    public event Func<IEventMessage, OnProcessEventArgs?, Task>? OnProcessAsync;
    /// <inheritdoc />
    public async Task WaitForEvents(CancellationToken cancellationToken = default)
    {
        await _processor.WaitForEvents(cancellationToken);
    }

    private async Task InternalOnProcessAsync(IEventMessage @event, OnProcessEventArgs? e)
    {
        var eventType = @event.GetType();
        var eventHandlerType = typeof(IEventMessageHandler<>).MakeGenericType(eventType);
        var handlerType = ReflectionCache.GetOrCreate(
            eventHandlerType
            , (t) =>
            {
                return AppDomain.CurrentDomain
                    .GetAssemblies()
                    .SelectMany(a => a.GetTypes())
                    .FirstOrDefault(t => eventHandlerType.IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface);
            });

        if (handlerType is null)
        {
            _logger.LogTrace("There are no handlers for the following event: {EventName}. Attempting OnProcessAsync delegate.", eventType.Name);

            if (OnProcessAsync is not null)
                await OnProcessAsync!.Invoke(@event, e);
            return;
        }

        // initiate a scope for the pipeline
        // - the scopes service provider is passed onto the eventhandler
#pragma warning disable IDE0063 // Use simple 'using' statement
        using (var scope = _serviceProvider.CreateScope())
        {
            // resolve the handler from the scope, if no handler is found, log and return
            var handler = ActivatorUtilities.GetServiceOrCreateInstance(scope.ServiceProvider, handlerType);
            if (handler == null)
            {
                _logger.LogTrace("There are no handlers for the following event: {EventName}", eventType.Name);
                return;
            }

            var pipeline = HostExtensions.BuildByRequestDelegate(
                async () => {
                    //var handler = ActivatorUtilities.CreateInstance(scope.ServiceProvider, handlerType);
                    
                    //NO - var handler = scope.ServiceProvider.GetService(eventHandlerType); // Requires that they are registered using DI
                    await (Task)eventHandlerType.GetMethod(nameof(IEventMessageHandler<IEventMessage>.Handle))!.Invoke(handler, [@event!, e?.CancellationToken])!;
                });

            await pipeline(new EventContext(@event, nameof(@event), scope.ServiceProvider));
        }
#pragma warning restore IDE0063 // Use simple 'using' statement
    }
}

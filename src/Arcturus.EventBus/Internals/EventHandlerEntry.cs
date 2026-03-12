namespace Arcturus.EventBus.Internals;

/// <summary>
/// Represents metadata describing an event handler, including its type, the event handler interface it implements, and
/// the method used to handle events.
/// </summary>
/// <remarks>This record is typically used to store and retrieve event handler metadata for event dispatching or
/// registration systems. It is intended for internal use within event handling infrastructure.</remarks>
/// <param name="HandlerType">The concrete type of the event handler. This type must implement the specified event handler interface.</param>
/// <param name="EventHandlerInterfaceType">The interface type that defines the contract for handling the event. Typically, this is a generic event handler
/// interface.</param>
/// <param name="HandleMethod">The method information representing the handler's event handling method. This method is invoked to process the
/// event.</param>
internal sealed record EventHandlerEntry(Type HandlerType, Type EventHandlerInterfaceType, MethodInfo HandleMethod);

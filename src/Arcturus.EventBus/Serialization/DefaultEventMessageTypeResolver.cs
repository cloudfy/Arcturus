namespace Arcturus.EventBus.Serialization;

/// <summary>
/// Default type resolver using the AppDomain.
/// </summary>
public class DefaultEventMessageTypeResolver(EventTypeRegistry eventTypeRegistry) // di injected
{
    /// <summary>
    /// Resolves a type by its name using the event type registry.
    /// </summary>
    /// <param name="typeName">The name of the type to resolve.</param>
    /// <returns>The resolved <see cref="Type"/> if found; otherwise, <c>null</c>.</returns>
    internal Type? ResolveType(string typeName)
    {
        ArgumentNullException.ThrowIfNull(typeName, nameof(typeName));
        return eventTypeRegistry.GetTypeByName(typeName);
    }
}

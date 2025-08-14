using Arcturus.EventBus.Abstracts;

namespace Arcturus.EventBus.Serialization;

/// <summary>
/// Default type resolver using the AppDomain.
/// </summary>
internal sealed class DefaultEventMessageTypeResolver
{
    private readonly Dictionary<string, Type?> _reflectionCache = [];

    /// <summary>
    /// Resolves <paramref name="typeName"/> to a <see cref="Type"/>.
    /// </summary>
    /// <param name="typeName">Required. Name of type to resolve.</param>
    /// <returns><see cref="Type"/> or null.</returns>
    /// <exception cref="InvalidOperationException">If more than one event has the same name.</exception>
    internal Type? ResolveType(string typeName)
    {
        ArgumentNullException.ThrowIfNull(typeName, nameof(typeName));

        if (_reflectionCache.TryGetValue(typeName, out var type))
            return type;

        type = AppDomain.CurrentDomain
            .GetAssemblies()
            .Select(a => a.GetType(typeName))
            .Where(_ => _ is not null)
            .FirstOrDefault();
        if (type is null)
        {
            // select single or default - an exception is thrown if more than one type is found
            type = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(a =>
                {
                    try { return a.GetTypes(); }
                    catch { return Type.EmptyTypes; } // Handle reflection errors gracefully
                })
                .Where(t => !t.IsInterface && !t.IsAbstract && typeof(IEventMessage).IsAssignableFrom(t))
                .FirstOrDefault(t =>
                    t.GetCustomAttribute<EventMessageAttribute>()?.Name == typeName);
        }
        // todo: implement more advanced type resolution (user delegates etc. from startup configuration)
        // - alternatively allow overriding the type resolver to a custom resolver.
        if (type is not null)
        {
            _reflectionCache.Add(typeName, type);
            return type;
        }
        return null;
    }
}

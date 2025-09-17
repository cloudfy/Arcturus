using Arcturus.EventBus.Abstracts;
using Arcturus.EventBus.Internals;

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

        // TODO: reduce the footprint by configuration (see https://github.com/cloudfy/Arcturus/issues/102)
        var matchingTypes = AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(a => a.GetTypesSafe()) // Use extension method for safe type retrieval
            .Where(t => t != null
                && !t.IsInterface
                && !t.IsAbstract
                && typeof(IEventMessage).IsAssignableFrom(t)
                && t.GetCustomAttribute<EventMessageAttribute>()?.Name == typeName)
            .ToArray();

        type = matchingTypes.Length switch
        {
            0 => null,
            1 => matchingTypes[0],
            _ => throw new UnprocessableEventException(
                $"Event named '{typeName}' is duplicated. Only one named event using EventMessageAttribute is allowed.")
        };


        // TODO: implement more advanced type resolution (user delegates etc. from startup configuration)
        // - alternatively allow overriding the type resolver to a custom resolver.
        // - https://github.com/cloudfy/Arcturus/issues/102
        if (type is not null)
        {
            _reflectionCache.Add(typeName, type);
            return type;
        }
        return null;
    }
}

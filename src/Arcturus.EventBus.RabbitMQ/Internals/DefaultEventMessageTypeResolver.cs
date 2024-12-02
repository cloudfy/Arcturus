namespace Arcturus.EventBus.RabbitMQ.Internals;

/// <summary>
/// Default type resolver using the AppDomain.
/// </summary>
internal class DefaultEventMessageTypeResolver
{
    private readonly Dictionary<string, Type?> _reflectionCache = [];

    /// <summary>
    /// Resolves <paramref name="typeName"/> to a <see cref="Type"/>.
    /// </summary>
    /// <param name="typeName">Required. Name of type to resolve.</param>
    /// <returns><see cref="Type"/> or null.</returns>
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
        if (type is not null)
        {
            _reflectionCache.Add(typeName, type);
            return type;
        }
        return null;
    }
}

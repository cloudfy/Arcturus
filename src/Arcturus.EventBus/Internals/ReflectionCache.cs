using System.Collections.Concurrent;

namespace Arcturus.EventBus.Internals;

internal static class ReflectionCache
{
    private static readonly ConcurrentDictionary<Type, Type> _cache = [];

    internal static Type? GetOrCreate(
        Type reflectedType
        , Func<Type, Type?> create)
    {
        if (_cache.TryGetValue(reflectedType, out Type? value))
            return value;

        var type = create(reflectedType);
        if (type is null)
            return null;

        _cache.TryAdd(reflectedType, type);
        return type;
    }
}
namespace Arcturus.EventBus.Internals;

internal static class ReflectionCache
{
    private static readonly object _lock = new();
    private static Dictionary<Type, Type> _cache = [];
    internal static Type? GetOrCreate(
        Type reflectedType
        , Func<Type, Type?> create)
    {
        if (_cache.ContainsKey(reflectedType))
            return _cache[reflectedType];

        lock (_lock)
        {
            var type = create(reflectedType);
            if (type is null)
                return null;

            _cache.Add(reflectedType, type);
            return type;
        }
    }
}
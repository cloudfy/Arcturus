namespace Arcturus.EventBus.Internals;

internal static class ReflectionCache
{
#if NET9_0_OR_GREATER
    private static readonly Lock _lock = new();
#else
    private static readonly object _lock = new();
#endif

    private static readonly Dictionary<Type, Type> _cache = [];

    internal static Type? GetOrCreate(
        Type reflectedType
        , Func<Type, Type?> create)
    {
        if (_cache.TryGetValue(reflectedType, out Type? value))
            return value;

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
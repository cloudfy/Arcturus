using System.Collections.Concurrent;
using System.Reflection;

namespace Arcturus.Patchable.Internals;

internal sealed class DefaultPatchTypeCache : IPatchTypeCache
{
    private readonly ConcurrentDictionary<Type, PropertyInfo[]> _propertyCache = new();

    public PropertyInfo[] GetProperties<T>()
    {
        if (_propertyCache.TryGetValue(typeof(T), out var properties))
            return properties;

        var props = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public);
        _propertyCache.TryAdd(typeof(T), props);

        return props;
    }
}

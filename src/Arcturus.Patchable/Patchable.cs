using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Arcturus.Patchable;

/// <summary>
/// Represents a dictionary that can be used to patch a class instance.
/// </summary>
/// <typeparam name="T">Type of patchable.</typeparam>
public class Patchable<T>
    : IDictionary<string, object?>
    where T : class, new()
{
    private readonly Dictionary<string, object?> _valueDictionary = [];
    public Patchable() { }

    public object? this[string key] 
    { 
        get => _valueDictionary[key];
        set => _valueDictionary[key] = value;
    }

    public ICollection<string> Keys => _valueDictionary.Keys;
    public ICollection<object?> Values => _valueDictionary.Values;
    public int Count => _valueDictionary.Count;
    public bool IsReadOnly => ((IDictionary<string, object?>)_valueDictionary).IsReadOnly;

    public void Add(string key, object? value) => _valueDictionary.Add(key, value);
    public void Add(KeyValuePair<string, object?> item) => _valueDictionary.Add(item.Key, item.Value);
    public void Clear() => _valueDictionary.Clear();
    public bool Contains(KeyValuePair<string, object?> item) => _valueDictionary.Contains(item);
    public bool ContainsKey(string key) => _valueDictionary.ContainsKey(key);
    public void CopyTo(KeyValuePair<string, object?>[] array, int arrayIndex)
    {
        ((IDictionary<string, object?>)_valueDictionary).CopyTo(array, arrayIndex);
    }

    public IEnumerator<KeyValuePair<string, object?>> GetEnumerator()
        => _valueDictionary.GetEnumerator();

    public bool Remove(string key)
        => _valueDictionary.Remove(key);

    public bool Remove(KeyValuePair<string, object?> item)
        => _valueDictionary.Remove(item.Key);

    public bool TryGetValue(string key, [MaybeNullWhen(false)] out object value)
        => _valueDictionary.TryGetValue(key, out value);
    
    IEnumerator IEnumerable.GetEnumerator()
        => _valueDictionary.GetEnumerator();
}

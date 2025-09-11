using Arcturus.Patchable.Internals;
using System.Reflection;
using System.Text.Json;

namespace Arcturus.Patchable;

/// <summary>
/// Applies a set of patch operations to a target object, modifying its properties based on the provided patchable data.
/// </summary>
public class PatchHandler : IPatchHandler
{
    private readonly PatchHandleOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="PatchHandler"/> class with the default patch handling options.
    /// </summary>
    public PatchHandler() : this(PatchHandleOptions.Default) { }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="PatchHandler"/> class with the specified options.
    /// </summary>
    /// <param name="options">The configuration options for the patch handler. This parameter must not be <c>null</c>. If the <see
    /// cref="PatchHandleOptions.TypeCache"/> property is <c>null</c>, a default type cache will be used.</param>
    public PatchHandler(PatchHandleOptions options)
    {
        _options = options;
        _options.TypeCache ??= new DefaultPatchTypeCache();
    }
    
    public R Apply<T, R>(Patchable<T> patchable, R targetObject)
        where T : class, new()
    {
        var properties = _options.TypeCache!.GetProperties<R>();

        foreach (var prop in patchable)
        {
            var propertyInfo = properties
                .FirstOrDefault(p => p.Name.Equals(prop.Key, StringComparison.OrdinalIgnoreCase));
            if (propertyInfo is not null)
            {
                var validProperty = _options.OnValidateProperty?.Invoke(propertyInfo, prop);
                if (validProperty.GetValueOrDefault(true) == false)
                {
                    continue;
                }

                var jsonElement = (JsonElement?)patchable[prop.Key];
                if (jsonElement is null)
                {
                    if (IsPropertyNullable(propertyInfo) == false)
                    {
                        throw new InvalidOperationException($"Property '{propertyInfo.Name}' does not allow null.");
                    }
                    propertyInfo.SetValue(targetObject, null);
                }
                else
                {
                    var valueValue = JsonHelper.ToObject(jsonElement.Value, propertyInfo.PropertyType);
                    propertyInfo.SetValue(targetObject, valueValue);
                }
            }
        }
        return targetObject;
    }

    public T? GetTypedValue<T>(Patchable<T> obj) where T : class, new()
    {
        var json = JsonSerializer.Serialize(obj.ValueDictionary);
        return JsonSerializer.Deserialize<T>(
            json, _options.JsonSerializerOptions ?? null);
    }

    public (bool Valid, string[]? InvalidProperties) IsValid<T>(Patchable<T> obj) where T : class, new()
    {
        var patchableProperties = _options.TypeCache!.GetProperties<T>();
        
        List<string> unknownProperties = [];

        foreach (var propertyName in obj.Keys)
        {
            var property = patchableProperties
                .FirstOrDefault(p => p.Name.Equals(propertyName, StringComparison.CurrentCultureIgnoreCase));

            if (property is null)
                unknownProperties.Add(propertyName);
        }
        if (unknownProperties.Count != 0)
            return (false, unknownProperties.ToArray());

        return (true, null);
    }

    private static bool IsPropertyNullable(PropertyInfo propertyInfo)
    {
        // Check if the property type is Nullable<T>
        var type = propertyInfo.PropertyType;
        if (Nullable.GetUnderlyingType(type) != null)
        {
            return true;
        }

        // For reference types with Nullable Reference Types (NRT) enabled
        if (!type.IsValueType)
        {
            var nullabilityContext = new NullabilityInfoContext();
            var nullabilityInfo = nullabilityContext.Create(propertyInfo);
            return nullabilityInfo.ReadState == NullabilityState.Nullable;
        }

        // Not nullable
        return false;
    }
}
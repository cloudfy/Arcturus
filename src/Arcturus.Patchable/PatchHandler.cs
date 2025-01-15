using Arcturus.Patchable.Internals;
using System.Reflection;
using System.Text.Json;

namespace Arcturus.Patchable;

public class PatchHandler : IPatchHandler
{
    public R Apply<T, R>(Patchable<T> patchable, R targetObject, PatchHandleOptions? options = null) 
        where T : class, new()
    {
        // TODO: cache and add support for nested properties
        var properties = typeof(R).GetProperties();
        
        foreach (var prop in patchable)
        {
            var propertyInfo = properties
                .FirstOrDefault(p => p.Name.Equals(prop.Key, StringComparison.OrdinalIgnoreCase));
            if (propertyInfo is not null)
            {
                var validProperty = options?.OnValidateProperty?.Invoke(propertyInfo, prop);
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
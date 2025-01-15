using System.Reflection;

namespace Arcturus.Patchable;

public class PatchHandleOptions
{
    /// <summary>
    /// Returns a value indicating whether the property should be applied.
    /// <para>
    /// Return false not to apply the property. Defaults to true.
    /// </para>
    /// </summary>
    public Func<PropertyInfo, KeyValuePair<string, object?>, bool>? OnValidateProperty = null;
}

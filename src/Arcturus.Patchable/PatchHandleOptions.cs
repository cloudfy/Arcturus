using Arcturus.Patchable.Internals;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Arcturus.Patchable;

/// <summary>
/// Represents options for configuring the behavior of patch handling operations.
/// </summary>
/// <remarks>This class provides settings to control how properties are validated, how JSON serialization and 
/// deserialization are performed, and how patch type information is cached. Use these options to  customize the
/// behavior of patch operations to suit your application's requirements.</remarks>
public class PatchHandleOptions
{
    /// <summary>
    /// Returns a value indicating whether the property should be applied.
    /// <para>
    /// Return false not to apply the property. Defaults to true.
    /// </para>
    /// </summary>
    public Func<PropertyInfo, KeyValuePair<string, object?>, bool>? OnValidateProperty = null;
    /// <summary>
    /// Gets or sets the options to configure the behavior of JSON serialization and deserialization.
    /// </summary>
    public JsonSerializerOptions? JsonSerializerOptions = null;
    /// <summary>
    /// Represents the cache used to store and retrieve patch type information.
    /// </summary>
    public IPatchTypeCache? TypeCache = null;

    internal static PatchHandleOptions Default => new()
    {
        JsonSerializerOptions = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
            , DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            , ReadCommentHandling = JsonCommentHandling.Skip
            , AllowTrailingCommas = true
        }
        , TypeCache = new DefaultPatchTypeCache()
    };
}

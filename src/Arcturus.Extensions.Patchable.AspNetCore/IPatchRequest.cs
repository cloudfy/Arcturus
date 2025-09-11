namespace Arcturus.Extensions.Patchable.AspNetCore;

/// <summary>
/// Represents a patch request that contains a collection of key-value pairs used to apply updates to an object.
/// </summary>
/// <remarks>This interface provides access to the type of the object being patched and allows enumeration of the
/// key-value pairs representing the updates.</remarks>
public interface IPatchRequest 
    : IEnumerable<KeyValuePair<string, object?>>
{
    /// <summary>
    /// Gets the type of the object being patched.
    /// </summary>
    Type Type { get; }
}
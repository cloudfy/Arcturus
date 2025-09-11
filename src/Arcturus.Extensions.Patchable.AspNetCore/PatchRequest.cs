using Arcturus.Patchable;

namespace Arcturus.Extensions.Patchable.AspNetCore;

/// <summary>
/// Represents a request to apply a set of patch operations to an object of type <typeparamref name="T"/>.
/// </summary>
/// <remarks>This class provides functionality for defining and enumerating patch operations on an object of type
/// <typeparamref name="T"/>. It implements <see cref="IPatchRequest"/> to expose the type being patched and <see
/// cref="IEnumerable{T}"/> to enumerate the patch operations.</remarks>
/// <typeparam name="T">The type of the object to which the patch operations will be applied. Must be a reference type with a parameterless
/// constructor.</typeparam>
public sealed class PatchRequest<T>
    : Patchable<T>, IPatchRequest, IEnumerable<KeyValuePair<string, object?>>
    where T : class, new()
{
    Type IPatchRequest.Type => typeof(T);
}

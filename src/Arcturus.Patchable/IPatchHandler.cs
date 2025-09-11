namespace Arcturus.Patchable;

/// <summary>
/// Defines methods for applying patch operations to objects and retrieving typed values from patchable objects.
/// </summary>
/// <remarks>This interface is designed to facilitate the application of patch operations to objects of various
/// types and to extract strongly-typed values from patchable objects. Implementations of this interface should ensure
/// that the patching process adheres to the constraints and expectations of the provided types.</remarks>
public interface IPatchHandler
{
    /// <summary>
    /// Applies the specified patch to the target object and returns the result.
    /// </summary>
    /// <typeparam name="T">The type of the object being patched. Must be a reference type with a parameterless constructor.</typeparam>
    /// <typeparam name="R">The type of the target object to which the patch is applied.</typeparam>
    /// <param name="patchable">The patchable object containing the changes to apply.</param>
    /// <param name="targetObject">The target object to which the patch will be applied.</param>
    /// <returns>The target object after the patch has been applied.</returns>
    R Apply<T, R>(Patchable<T> patchable, R targetObject) where T : class, new();
    /// <summary>
    /// Retrieves the typed value from the specified <see cref="Patchable{T}"/> object.
    /// </summary>
    /// <typeparam name="T">The type of the value to retrieve. Must be a reference type with a parameterless constructor.</typeparam>
    /// <param name="obj">The <see cref="Patchable{T}"/> object from which to retrieve the value.</param>
    /// <returns>The value of type <typeparamref name="T"/> if it is set; otherwise, <see langword="null"/>.</returns>
    T? GetTypedValue<T>(Patchable<T> obj) where T : class, new();
    /// <summary>
    /// Determines whether the specified <see cref="Patchable{T}"/> object is valid and identifies any invalid
    /// properties.
    /// </summary>
    /// <typeparam name="T">The type of the object being patched. Must be a reference type with a parameterless constructor.</typeparam>
    /// <param name="obj">The <see cref="Patchable{T}"/> object to validate.</param>
    /// <returns>A tuple containing a boolean value indicating whether the object is valid and an array of strings representing
    /// the names of invalid properties, if any. If the object is valid, the array of invalid properties will be null.</returns>
    (bool Valid, string[]? InvalidProperties) IsValid<T>(Patchable<T> obj) where T : class, new();
}
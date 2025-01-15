namespace Arcturus.Patchable;

public interface IPatchHandler
{
    /// <summary>
    /// Applies <paramref name="patchable"/> to <paramref name="targetObject"/>.
    /// <para>
    /// Properties much be aligned by naming.
    /// </para>
    /// <para>
    /// A <see cref="InvalidOperationException" /> is thrown if a property does not allow null.
    /// </para>
    /// <para>
    /// Use <see cref="IPatchHandler"/> for dependency injection.
    /// </para>
    /// </summary>
    /// <typeparam name="T">Type of the patchable.</typeparam>
    /// <typeparam name="R">Type of the response.</typeparam>
    /// <param name="patchable"><see cref="Patchable{T}"/></param>
    /// <param name="targetObject"><typeparamref name="R"/></param>
    /// <param name="options">Optional. Options used for application.</param>
    /// <returns><typeparamref name="R"/></returns>
    /// <exception cref="InvalidOperationException" />
    R Apply<T, R>(Patchable<T> patchable, R targetObject, PatchHandleOptions? options = null) where T : class, new();
}
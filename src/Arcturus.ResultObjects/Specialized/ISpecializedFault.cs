using System.Net;

namespace Arcturus.ResultObjects.Specialized;

/// <summary>
/// Abstraction for specialized fault.
/// </summary>
public interface ISpecializedFault
{
    /// <summary>
    /// Gets the status code of the fault to assign to the <see cref="Result{T}"/>.
    /// </summary>
    HttpStatusCode HttpStatusCode { get; }
}
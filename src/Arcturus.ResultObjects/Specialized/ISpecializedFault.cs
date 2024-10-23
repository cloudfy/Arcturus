using System.Net;

namespace Arcturus.ResultObjects.Specialized;

public interface ISpecializedFault
{
    HttpStatusCode HttpStatusCode { get; }
}
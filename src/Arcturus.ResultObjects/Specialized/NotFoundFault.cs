using System.Net;

namespace Arcturus.ResultObjects.Specialized;

public record NotFoundFault(string Code, string Message)
    : Fault(Code, Message), ISpecializedFault
{
    public HttpStatusCode HttpStatusCode => HttpStatusCode.NotFound;
}

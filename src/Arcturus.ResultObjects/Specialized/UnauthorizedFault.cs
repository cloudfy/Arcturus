using System.Net;

namespace Arcturus.ResultObjects.Specialized;

public record UnauthorizedFault(string Code, string Message)
    : Fault(Code, Message), ISpecializedFault
{
    public HttpStatusCode HttpStatusCode => HttpStatusCode.Unauthorized;
}

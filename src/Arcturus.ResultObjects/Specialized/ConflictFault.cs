using System.Net;

namespace Arcturus.ResultObjects.Specialized;

public record ConflictFault(string Code, string Message)
    : Fault(Code, Message), ISpecializedFault
{
    public HttpStatusCode HttpStatusCode => HttpStatusCode.Conflict;
}

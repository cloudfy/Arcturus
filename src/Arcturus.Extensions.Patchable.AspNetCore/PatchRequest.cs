using Arcturus.Patchable;

namespace Arcturus.Extensions.Patchable.AspNetCore;

public sealed class PatchRequest<T> 
    : Patchable<T>, IPatchRequest, IEnumerable<KeyValuePair<string, object?>>
    where T : class, new()
{
    Type IPatchRequest.Type => typeof(T);
}

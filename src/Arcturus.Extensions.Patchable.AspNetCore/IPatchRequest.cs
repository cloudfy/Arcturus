namespace Arcturus.Extensions.Patchable.AspNetCore;

public interface IPatchRequest : IEnumerable<KeyValuePair<string, object?>>
{
    Type Type { get; }
}
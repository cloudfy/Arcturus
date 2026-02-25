namespace Arcturus.Extensions.ResultObjects.AspNetCore;

public class ResultObjectOptions
{
    public bool RegisterProblemDetailsFactory { get; set; } = true;
    public Action<IDictionary<int, (string Title, string Link)>>? ValidateClientMappings = null;
}

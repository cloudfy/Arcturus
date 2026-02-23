using Arcturus.DevHost.Hosting.Abstracts;

namespace Arcturus.DevHost.Hosting;

public class ProjectResource(
    IProjectMetadata metadata) 
    : IResource
{
    public IProjectMetadata Metadata { get; } = metadata;
    public string[]? Urls { get; internal set; }
    public Dictionary<string, string>? EnvironmentVariables { get; internal set; }
}

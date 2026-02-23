using Arcturus.DevHost.Hosting.Abstracts;
using Microsoft.Extensions.Logging;

namespace Arcturus.DevHost.Hosting;

public sealed class OrchestratorHostBuilder
{
    private readonly List<IResource> _resources = [];
    private readonly OrchestratorOptions _options = new();
    private Action<ILoggingBuilder>? _configureLogging;

    public static OrchestratorHostBuilder Create() => new();

    public OrchestratorHostBuilder AddProject<T>(
        Action<ProjectResource>? configure = null) 
        where T : IProjectMetadata, new()
    {
        var metadata = new T();
        var resource = new ProjectResource(metadata);
        configure?.Invoke(resource);
        _resources.Add(resource);
        return this;
    }

    public OrchestratorHostBuilder Configure(
        Action<OrchestratorOptions> configure)
    {
        configure(_options);
        return this;
    }
    public OrchestratorHostBuilder ConfigureLogging(Action<ILoggingBuilder> configure)
    {
        _configureLogging = configure;
        return this;
    }

    /// <summary>
    /// For "npm run dev": AddExecutable("npm", "run dev", workingDirectory: "path/to/app");
    /// </summary>
    public OrchestratorHostBuilder AddExecutable(string fileName, string arguments = "", string? workingDirectory = null, bool? useShellExecute = false)
    {
        _resources.Add(new ExecutableResource(fileName, arguments, workingDirectory, useShellExecute));
        return this;
    }

    public OrchestratorHost Build() => new(_resources, _configureLogging);
}
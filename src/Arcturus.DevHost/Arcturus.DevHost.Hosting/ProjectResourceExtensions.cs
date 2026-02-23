namespace Arcturus.DevHost.Hosting;

public static class ProjectResourceExtensions
{
    public static ProjectResource WithUrls(this ProjectResource resource, params string[] urls)
    {
        resource.Urls = urls;
        return resource;
    }
    public static ProjectResource WithEnvironmentVariables(this ProjectResource resource, Dictionary<string, string> envVars)
    {
        resource.EnvironmentVariables = envVars;
        return resource;
    }
    public static ProjectResource WithEnvironmentVariable(this ProjectResource resource, string key, string value)
    {
        resource.EnvironmentVariables ??= [];
        resource.EnvironmentVariables[key] = value;
        return resource;
    }
}
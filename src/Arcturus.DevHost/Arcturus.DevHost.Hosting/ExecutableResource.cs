using Arcturus.DevHost.Hosting.Abstracts;

namespace Arcturus.DevHost.Hosting;

public class ExecutableResource : IResource
{
    public string FileName { get; }
    public string Arguments { get; }
    public string? WorkingDirectory { get; }
    public bool? UseShellExecute { get; }

    internal ExecutableResource(string fileName, string arguments, string? workingDirectory, bool? useShellExecute)
    {
        FileName = fileName;
        Arguments = arguments;
        WorkingDirectory = workingDirectory;
        UseShellExecute = useShellExecute;
    }
}

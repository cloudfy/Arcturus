namespace Arcturus.DevHost.Hosting;

public sealed class OrchestratorOptions
{
    public bool ForwardOutput { get; set; } = true;
    public Dictionary<string, string> GlobalEnvironment { get; } = [];
}

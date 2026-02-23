using Arcturus.DevHost.Hosting.Abstracts;
using Arcturus.DevHost.Hosting.Internals;
using Arcturus.DevHost.Hosting.Interop;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Arcturus.DevHost.Hosting;

public sealed class OrchestratorHost
{
    private readonly List<IResource> _resources;
    private readonly List<Process> _processes = [];
    private readonly List<Task> _tasks = [];
    private readonly JobObjectHost _jobObjectHost;
    
    private CancellationTokenSource? _cts;
    private ILogger<OrchestratorHost>? _logger;
    private readonly Action<ILoggingBuilder>? _configureLogging;

    internal OrchestratorHost(List<IResource> resources, Action<ILoggingBuilder>? configureLogging)
    {
        _resources = resources;
        _configureLogging = configureLogging;

        // Set up logging infrastructure
        if (_configureLogging != null)
        {
            var services = new ServiceCollection();
            var loggingBuilder = services.AddLogging(builder =>
            {
                _configureLogging(builder);
            });

            var serviceProvider = services.BuildServiceProvider();
            _logger = serviceProvider.GetRequiredService<ILogger<OrchestratorHost>>();
        }

        _jobObjectHost = new JobObjectHost(_logger);
    }

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _jobObjectHost.CreateJobObject();

        Console.CancelKeyPress += OnCancelKeyPress;
        _logger?.LogInformation($"[OrchestratorHost] Registered shutdown handlers. PID: {Environment.ProcessId}");

        foreach (var res in _resources)
        {
            if (res is ExecutableResource er)
            {
                bool useShellExecute = er.UseShellExecute ?? false;

                var psi = new ProcessStartInfo
                {
                    FileName = er.FileName,
                    Arguments = er.Arguments,
                    WorkingDirectory = er.WorkingDirectory ?? Environment.CurrentDirectory,
                    UseShellExecute = useShellExecute  // For npm etc.
                    // redirect plus environment variables can be added here if needed
                };
                //var userPath = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.User) ?? "";
                //var machinePath = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Machine) ?? "";
                //psi.EnvironmentVariables["PATH"] = $"{userPath};{machinePath}";
                // Copy all environment variables, not just PATH
                if (!useShellExecute) 
                { 
                    foreach (System.Collections.DictionaryEntry envVar in Environment.GetEnvironmentVariables())
                    {
                        psi.EnvironmentVariables[(string)envVar.Key] = (string?)envVar.Value;
                    }
                }

                _logger?.LogDebug($"Starting process: {er.FileName} {er.Arguments}");
                
                try
                {
                    var p = Process.Start(psi);
                    _logger?.LogDebug($"Started process {er.FileName} with PID {p?.Id}");
                    if (p != null)
                    {
                        // Add to job object - will be killed when this process dies
                        _jobObjectHost.AssignProcessToJobObject(p);

                        _processes.Add(p);
                        _tasks.Add(MonitorProcessAsync(p, _cts.Token));
                    }
                }
                catch (Exception e)
                {
                    _logger?.LogDebug(e.Message);
                }
            }
            else if (res is ProjectResource pr)
            {
                var r = BuildDotNetProjectTask(pr, _cts.Token);
                _tasks.Add(r);
            }
        }

        try
        {
            await Task.WhenAll(_tasks);
        }
        catch (OperationCanceledException)
        {
            _logger?.LogInformation("[OrchestratorHost] RunAsync was cancelled");
        }
        catch (Exception ex)
        {
            _logger?.LogWarning($"[OrchestratorHost] Error in RunAsync: {ex.Message}");
        }
        finally
        {
            _logger?.LogInformation("[OrchestratorHost] Cleanup starting");
            await CleanupAsync();
            
            Console.CancelKeyPress -= OnCancelKeyPress;
            _cts?.Dispose();
            _jobObjectHost?.Dispose();
        }
    }

    private void OnCancelKeyPress(object? sender, ConsoleCancelEventArgs e)
    {
        _logger?.LogInformation("[OrchestratorHost] Ctrl+C pressed - cancelling...");
        e.Cancel = true;
        _cts?.Cancel();
    }

    private async Task MonitorProcessAsync(Process process, CancellationToken cancellationToken)
    {
        try
        {
            await process.WaitForExitAsync(cancellationToken);
        }
        catch (OperationCanceledException)
        {
            // Cancelled - will be killed in cleanup
        }
    }

    private async Task CleanupAsync()
    {
        // Kill all spawned processes
        foreach (var p in _processes)
        {
            try
            {
                if (!p.HasExited)
                {
                    p.Kill(entireProcessTree: true);
                    
                    using var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(3));
                    await p.WaitForExitAsync(timeoutCts.Token);
                }
            }
            catch
            {
                // Best effort
            }
            finally
            {
                p.Dispose();
            }
        }
    }

    public void Run() => RunAsync().GetAwaiter().GetResult();

    private Task BuildDotNetProjectTask(ProjectResource projectResource, CancellationToken cancellationToken)
    {
        var currentDir = AppDomain.CurrentDomain.BaseDirectory;
        var projectMetadata = projectResource.Metadata;
        var assemblyName = projectMetadata.GetAssemblyName();
        var apiDllPath = Path.Combine(currentDir, $"{assemblyName}.dll");

        if (!File.Exists(apiDllPath))
        {
            _logger?.LogWarning($"Could not find {assemblyName}.dll at {apiDllPath}");
            return Task.CompletedTask;
        }

        return Task.Run(async () =>
        {
            try
            {
                _logger?.LogInformation($"Loading {projectMetadata.ProjectName} from {apiDllPath}");

                foreach (var kvp in projectResource.EnvironmentVariables ?? [])
                {
                    Environment.SetEnvironmentVariable(kvp.Key, kvp.Value);
                }

                var apiAssembly = System.Reflection.Assembly.LoadFrom(apiDllPath);
                var entryPoint = apiAssembly.EntryPoint;

                if (entryPoint != null)
                {
                    _logger?.LogInformation($"Starting {projectMetadata.ProjectName}...");

                    List<string> argList = new();
                    if (projectResource.Urls != null && projectResource.Urls.Length > 0)
                    {
                        var urls = string.Join(";", projectResource.Urls);
                        Environment.SetEnvironmentVariable("ASPNETCORE_URLS", urls);
                        argList.Add("--urls");
                        argList.Add(urls);
                    }

                    var result = entryPoint.Invoke(null, [argList.ToArray()]);

                    if (result is Task task)
                    {
                        // Use a linked token that responds to both cancellation and completion
                        await task.WaitAsync(cancellationToken);
                    }
                }
                else
                {
                    _logger?.LogWarning($"Could not find entry point for {projectMetadata.ProjectName}");
                }
            }
            catch (OperationCanceledException)
            {
                _logger?.LogInformation($"{projectMetadata.ProjectName} was cancelled");
            }
            catch (Exception ex)
            {
                _logger?.LogError($"Error running {projectMetadata.ProjectName}: {ex}");
            }
        }, cancellationToken);
    }
}
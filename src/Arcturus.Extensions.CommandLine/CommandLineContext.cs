using Arcturus.CommandLine.Abstractions;

namespace Arcturus.CommandLine;

/// <summary>
/// Encapsulates all command-line specific information about an individual execution.
/// </summary>
public class CommandLineContext
{
    /// <summary>
    /// Gets or sets the <see cref="IServiceProvider"/> that provides access to the application's service container.
    /// </summary>
    public required IServiceProvider Services { get; init; }

    /// <summary>
    /// Gets or sets the command-line arguments.
    /// </summary>
    public required string[] Args { get; init; }

    /// <summary>
    /// Gets or sets the <see cref="CancellationToken"/> for the execution.
    /// </summary>
    public CancellationToken CancellationToken { get; init; }

    /// <summary>
    /// Gets or sets the root command instance.
    /// </summary>
    public required IAbstractCommand RootCommand { get; init; }

    /// <summary>
    /// Gets a key/value collection that can be used to share data within the scope of this execution.
    /// </summary>
    public IDictionary<object, object?> Items { get; } = new Dictionary<object, object?>();
}

namespace Arcturus.CommandLine.Abstractions;

/// <summary>
/// A function that can process a command-line execution.
/// </summary>
/// <param name="context">The <see cref="CommandLineContext"/> for the current execution.</param>
/// <returns>A task that represents the completion of command processing.</returns>
public delegate Task CommandLineDelegate(CommandLineContext context);

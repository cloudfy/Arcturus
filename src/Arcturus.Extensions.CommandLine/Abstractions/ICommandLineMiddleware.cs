namespace Arcturus.CommandLine.Abstractions;

/// <summary>
/// Defines middleware for the command-line execution pipeline.
/// </summary>
public interface ICommandLineMiddleware
{
    /// <summary>
    /// Request handling method.
    /// </summary>
    /// <param name="context">The <see cref="CommandLineContext"/> for the current execution.</param>
    /// <param name="next">The delegate representing the remaining middleware in the pipeline.</param>
    /// <returns>A <see cref="Task"/> that represents the execution of this middleware.</returns>
    Task InvokeAsync(CommandLineContext context, CommandLineDelegate next);
}

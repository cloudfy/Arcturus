namespace Arcturus.CommandLine.Abstractions;

/// <summary>
/// Defines a commandline command handler. 
/// <para>
/// Register a handler using <see cref="ServiceCollectionExtensions.AutoRegisterCommandHandlers(Microsoft.Extensions.DependencyInjection.IServiceCollection, Microsoft.Extensions.DependencyInjection.ServiceLifetime)"/> or 
/// using manual registration via AddTransient().
/// </para>
/// </summary>
/// <typeparam name="TCommand"><see cref="ICommand"/> implementation.</typeparam>
public interface ICommandHandler<TCommand>
    where TCommand : ICommand
{
    /// <summary>
    /// Handles the command.
    /// </summary>
    /// <param name="command">Command to handle.</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    ValueTask Handle(TCommand command, CancellationToken cancellationToken);
}
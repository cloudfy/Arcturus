using Arcturus.CommandLine.Abstractions;
using Arcturus.Extensions.CommandLine.Internals;
using Microsoft.Extensions.DependencyInjection;

namespace Arcturus.CommandLine;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddCommandLine(this IServiceCollection services)
    {
        services.AddSingleton<CommandLineBuilderConfiguration>();
        AutoRegisterCommandHandlers(services);
        return services;
    }
    /// <summary>
    /// Registers all command handlers in the executing assembly.
    /// <para>
    /// Alternative is to register each command handler individually using AddTransient().
    /// </para>
    /// </summary>
    /// <param name="services">Required.</param>
    /// <param name="serviceLifetime">Specifies the <see cref="ServiceLifetime"/> of each command handler.</param>
    /// <returns><see cref="IServiceCollection"/></returns>
    private static IServiceCollection AutoRegisterCommandHandlers(
        this IServiceCollection services, ServiceLifetime serviceLifetime = ServiceLifetime.Transient)
    {
        var handlerTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => t.GetInterfaces().Any(i =>
                i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommandHandler<>)))
            .ToList();

        foreach (var handlerType in handlerTypes)
        {
            var commandInterface = handlerType.GetInterfaces()
                .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommandHandler<>));

            services.AddTransient(commandInterface, handlerType);
        }

        return services;
    }
}

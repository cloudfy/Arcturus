using Arcturus.EventBus.Abstracts;

namespace Arcturus.EventBus.Internals;

internal static class AssemblyExtensions
{
    internal static Type[] GetTypesSafe(this Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException ex)
        {
            return [.. ex.Types.Where(t => t is not null)!];
        }
        catch
        {
            return Type.EmptyTypes;
        }
    }


    internal static IEventMessage[] GetEventMessagesFromAssemblies()
    {
        return AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(a => GetEventMessagesFromAssembly(a))
            .ToArray();
    }

    internal static IEventMessage[] GetEventMessagesFromAssembly(Assembly assembly)
    {
        return assembly
            .GetTypesSafe().Where(t => t != null
                && !t.IsInterface
                && !t.IsAbstract
                && typeof(IEventMessage).IsAssignableFrom(t))
            .Cast<IEventMessage>()
            .ToArray();
    }

    /// <summary>
    /// Gets all event message types (not instances) from an assembly.
    /// </summary>
    /// <param name="assembly">The assembly to scan.</param>
    /// <returns>Array of types that implement IEventMessage.</returns>
    internal static Type[] GetEventMessageTypesFromAssembly(Assembly assembly)
    {
        return assembly
            .GetTypesSafe()
            .Where(t => t != null
                && !t.IsInterface
                && !t.IsAbstract
                && typeof(IEventMessage).IsAssignableFrom(t))
            .ToArray();
    }

    /// <summary>
    /// Gets all event handler types from an assembly.
    /// </summary>
    /// <param name="assembly">The assembly to scan.</param>
    /// <returns>Array of types that implement IEventMessageHandler&lt;T&gt;.</returns>
    internal static Type[] GetEventHandlerTypesFromAssembly(Assembly assembly)
    {
        return assembly
            .GetTypesSafe()
            .Where(t => t != null
                && !t.IsInterface
                && !t.IsAbstract
                && t.GetInterfaces().Any(i =>
                    i.IsGenericType &&
                    i.GetGenericTypeDefinition() == typeof(IEventMessageHandler<>)))
            .ToArray();
    }
}

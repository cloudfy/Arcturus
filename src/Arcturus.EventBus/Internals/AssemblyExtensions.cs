using Arcturus.EventBus.Abstracts;

namespace Arcturus.EventBus.Internals;

internal static class AssemblyExtensions
{
    private static List<Assembly>? _assemblyCache = null;

    internal static Type[] GetTypesSafe(this Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException ex)
        {
            return ex.Types.Where(t => t != null).ToArray();
        }
        catch
        {
            return Type.EmptyTypes;
        }
    }

    internal static Assembly[] LoadAllAssemblies()
    {
        if (_assemblyCache is not null)
            return [.. _assemblyCache];

        _assemblyCache = [];
        var loadedPaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            try
            {
                if (!assembly.IsDynamic && !string.IsNullOrEmpty(assembly.Location))
                {
                    loadedPaths.Add(assembly.Location);
                    _assemblyCache.Add(assembly);
                }
            }
            catch
            {
                // Skip assemblies that throw exceptions
            }
        }

        var baseDirectory = AppContext.BaseDirectory;
        if (!string.IsNullOrEmpty(baseDirectory) && Directory.Exists(baseDirectory))
        {
            var dllFiles = Directory.GetFiles(baseDirectory, "*.dll", SearchOption.TopDirectoryOnly);

            foreach (var dllFile in dllFiles)
            {
                if (loadedPaths.Contains(dllFile))
                {
                    continue;
                }

                try
                {
                    var assembly = Assembly.LoadFrom(dllFile);
                    loadedPaths.Add(dllFile);
                    _assemblyCache.Add(assembly);
                }
                catch
                {
                    // Skip files that cannot be loaded as assemblies
                }
            }
        }

        return [.. _assemblyCache];
    }

    /// <summary>
    /// Retrieves all concrete types from the specified assembly that implement the IEventMessageHandler<T> interface.
    /// </summary>
    /// <remarks>This method excludes interfaces and abstract classes, returning only concrete types that
    /// implement the generic IEventMessageHandler<T> interface. Use this method to discover event handler
    /// implementations for event bus registration or reflection-based processing.</remarks>
    /// <param name="assembly">The assembly from which to search for event handler types. Cannot be null.</param>
    /// <returns>An array of Type objects representing the concrete event handler types found in the specified assembly. The
    /// array is empty if no matching types are found.</returns>
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
    /// <summary>
    /// Retrieves all event message types defined in the specified assembly, along with their associated names.
    /// </summary>
    /// <remarks>Only types that implement the IEventMessage interface and are not abstract or interfaces are
    /// included. If an event message type is decorated with the EventMessageAttribute, its name is used; otherwise, the
    /// full name of the type is returned.</remarks>
    /// <param name="assembly">The assembly from which to retrieve event message types. This parameter must not be null.</param>
    /// <returns>An enumerable collection of tuples, each containing the name of the event message and its corresponding type.</returns>
    internal static IEnumerable<(string, Type)> GetEventMessageTypesFromAssembly(Assembly assembly)
    {
        var eventMessageTypes = assembly
            .GetTypesSafe()
            .Where(t => t != null
                && !t.IsInterface
                && !t.IsAbstract
                && typeof(IEventMessage).IsAssignableFrom(t))
            .ToArray();

        foreach (var eventMessageType in eventMessageTypes)
        {
            var eventAttr = eventMessageType.GetCustomAttribute<EventMessageAttribute>(true);
            if (eventAttr is not null)
            {
                yield return (eventAttr.Name, eventMessageType);
            }
            else
            {
                yield return (eventMessageType.FullName!, eventMessageType);
            }
        }
    }
}

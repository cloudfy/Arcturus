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
            return ex.Types.Where(t => t != null).ToArray();
        }
        catch
        {
            return Type.EmptyTypes;
        }
    }

    internal static Assembly[] LoadAllAssemblies()
    {
        var loadedAssemblies = new List<Assembly>();
        var loadedPaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            try
            {
                if (!assembly.IsDynamic && !string.IsNullOrEmpty(assembly.Location))
                {
                    loadedPaths.Add(assembly.Location);
                    loadedAssemblies.Add(assembly);
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
                    loadedAssemblies.Add(assembly);
                }
                catch
                {
                    // Skip files that cannot be loaded as assemblies
                }
            }
        }

        return [.. loadedAssemblies];
    }
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

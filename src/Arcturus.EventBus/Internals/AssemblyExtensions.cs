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
}

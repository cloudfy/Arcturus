namespace Arcturus.Patchable;

public interface IPatchTypeCache
{
    System.Reflection.PropertyInfo[] GetProperties<T>();
}

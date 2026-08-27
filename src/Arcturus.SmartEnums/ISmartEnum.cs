namespace Arcturus.SmartEnums;

public interface ISmartEnum<T>
    where T : ISmartEnum<T>
{
    string Value { get; }

    static abstract T FromValue(string value);

    static abstract bool TryFromValue(
        string value,
        out T? result);
}

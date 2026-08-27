namespace Arcturus.SmartEnums;

public abstract record SmartEnum<T>
        where T : SmartEnum<T>
{
    public string Value { get; }

    protected SmartEnum(string value)
    {
        Value = value;
    }

    public override string ToString() => Value;
}

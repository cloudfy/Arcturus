namespace Arcturus.CommandLine.Abstractions;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class OptionAttribute(
    string name
    , string? description) : Attribute
{
    public string Name => name;
    public string? Description => description;
}

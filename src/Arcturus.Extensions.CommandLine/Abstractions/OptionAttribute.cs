namespace Arcturus.CommandLine.Abstractions;


[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class OptionAttribute(
    string name
    , string? description
    , string[]? alias = null) : Attribute
{
    public OptionAttribute(
        string name
        , string? description
        , bool required) 
        : this(name, description)
    {
        Required = required;
    }
    public string Name => name;
    public string? Description => description;
    public bool? Required { get; }
    public string[]? Aliases => alias;
}
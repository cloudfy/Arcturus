namespace Arcturus.CommandLine.Abstractions;

/// <summary>
/// Represents a command attribute.
/// </summary>
/// <param name="name">Required. Name of the comman, ex. 'cli {name}'.</param>
/// <param name="description">Optional. A description of the command to provide help.</param>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class CommandAttribute(
    string name
    , string? description) : Attribute
{
    public string Name => name;
    public string? Description => description;
}

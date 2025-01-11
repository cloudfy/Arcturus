namespace Arcturus.EventBus.Abstracts;

/// <summary>
/// Specifies properties of the integration event.
/// <para>
/// Use the <see cref="EventMessageAttribute"/> to override the name of the event.
/// </para>
/// </summary>
/// <param name="name">Required. Name of the event for binding.</param>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class EventMessageAttribute(string name) : Attribute
{
    /// <summary>
    /// Gets the name of the event.
    /// </summary>
    public string Name { get; } = name;
}
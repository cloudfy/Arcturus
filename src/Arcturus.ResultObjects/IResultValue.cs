namespace Arcturus.ResultObjects;

/// <summary>
/// Defines a contract for accessing the underlying value of a result object.
/// </summary>
/// <remarks>Implementations of this interface provide a way to retrieve the value associated with a result, which
/// may represent a successful outcome, an error, or another result state. The meaning and type of the value depend on
/// the specific implementation.</remarks>
public interface IResultValue
{
    /// <summary>
    /// Gets the underlying value represented by the current instance.
    /// </summary>
    object? Value { get; }
}
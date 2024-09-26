namespace Arcturus.ResultObjects;

/// <summary>
/// Reprensents a fault object. Acts as the cause of the failure from <see cref="Result.Failure(Arcturus.ResultObjects.Fault?)"/>.
/// </summary>
/// <param name="Code">Optional. Gets a code of the fault.</param>
/// <param name="Message">Required. Gets a message of the fault.</param>
public record Fault(string? Code, string Message)
{
    /// <summary>
    /// Returns an instance of <see cref="Fault"/> by <paramref name="code"/> and <paramref name="message"/>.
    /// </summary>
    /// <param name="code">Required. String of code.</param>
    /// <param name="message">Required. String of message.</param>
    /// <returns></returns>
    public static Fault From(string code, string message) => new(code, message);
    /// <summary>
    /// Returns an instance of <see cref="Fault"/> by <paramref name="message"/> only.
    /// </summary>
    /// <param name="message">Required. String of message.</param>
    /// <returns></returns>
    public static Fault From(string message) => new(null, message);
}
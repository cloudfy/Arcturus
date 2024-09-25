namespace Arcturus.ResultObjects;

/// <summary>
/// Reprensents a fault object. Acts as the cause of the failure from <see cref="Result.Failure(Arcturus.ResultObjects.Fault?)"/>.
/// </summary>
/// <param name="Code">Gets a code of the fault.</param>
/// <param name="Message">Gets a message of the fault.</param>
public record Fault(string Code, string Message)
{
}
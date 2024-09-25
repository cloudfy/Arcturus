namespace Arcturus.ResultObjects;

/// <summary>
/// Represents a result of an operation - success or failure.
/// </summary>
/// <typeparam name="T">Type fo containing result.</typeparam>
public sealed class Result<T> : Result
{
    internal Result(bool isSuccess, T? value, Fault? fault = null) : base(isSuccess, fault)
    {
        Value = value;
    }
    /// <summary>
    /// Returns value of result or null.
    /// </summary>
    public T? Value { get; }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    public static implicit operator Result<T>(T? value) => new(true, value);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="failure"></param>
    public static implicit operator Result<T>(Fault failure) => new(false, default, failure);
}

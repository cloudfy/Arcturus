namespace Arcturus.ResultObjects;

/// <summary>
/// Represents a result of an operation - success or failure.
/// </summary>
/// <typeparam name="T">Type fo containing result.</typeparam>
public class Result<T> : Result
{
    /// <summary>
    /// Creates a new instance of <see cref="Result{T}"/> with a <paramref name="fault"/> and <paramref name="isSuccess"/>.
    /// </summary>
    /// <param name="isSuccess">A value of <see cref="bool"/> indicating is the result represens success.</param>
    /// <param name="value">Optional value of <typeparamref name="T"/> to assign.</param>
    /// <param name="fault">Optional <see cref="ResultObjects.Fault"/> if <paramref name="isSuccess"/> is false.</param>
    protected Result(bool isSuccess, T? value, Fault? fault = null) : base(isSuccess, fault)
    {
        Value = value;
    }

    internal static Result<T> Create(bool isSuccess, T? value, Fault? fault = null) => new(isSuccess, value, fault);

    /// <summary>
    /// Returns value of result or null.
    /// </summary>
    public T? Value { get; }
    /// <summary>
    /// Returns a <see cref="Result{T}"/> as a success of <paramref name="value"/>.
    /// </summary>
    /// <param name="value">Value of result object.</param>
    public static implicit operator Result<T>(T? value) => new(true, value);
    /// <summary>
    /// Returns a <see cref="Result{T}"/> as a fault of <paramref name="failure"/>.
    /// </summary>
    /// <param name="failure">Fault of failure result.</param>
    public static implicit operator Result<T>(Fault failure) => new(false, default, failure);
}

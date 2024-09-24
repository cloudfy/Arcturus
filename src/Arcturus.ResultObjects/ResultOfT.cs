namespace Arcturus.ResultObjects;

public sealed class Result<T> : Result
{
    internal Result(bool isSuccess, T? value) : base(isSuccess)
    {
        Value = value;
    }

    public T? Value { get; }
}

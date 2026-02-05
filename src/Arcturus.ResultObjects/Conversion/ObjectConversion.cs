using System.Diagnostics.CodeAnalysis;

namespace Arcturus.ResultObjects.Conversion;

public static class ObjectConversion
{
    /// <summary>
    /// Attempts to retrieve the success value from the specified result.
    /// </summary>
    /// <remarks>Use this method to safely access the value of a successful result without throwing an
    /// exception if the result is not successful.</remarks>
    /// <typeparam name="T">The type of the value contained in the result on success.</typeparam>
    /// <param name="result">The result to extract the success value from.</param>
    /// <param name="value">When this method returns, contains the value if the result represents success; otherwise, the default value for
    /// the type.</param>
    /// <returns>true if the result represents success and the value was retrieved; otherwise, false.</returns>
    public static bool TryGetSuccessValue<T>(this Result<T> result, [NotNullWhen(true)] out T? value)
    {
        if (result.IsSuccess)
        {
            value = result.Value!;
            return true;
        }
        value = default!;
        return false;
    }
    /// <summary>
    /// Creates a successful result containing the specified value.
    /// </summary>
    /// <typeparam name="T">The type of the value to include in the result.</typeparam>
    /// <param name="value">The value to include in the successful result.</param>
    /// <returns>A Result<T> instance representing a successful operation with the specified value.</returns>
    public static Result<T> ToSuccessResult<T>(this T value)
    {
        return Result<T>.Success<T>(value);
    }
    /// <summary>
    /// Creates a failed result containing the specified fault for the given value.
    /// </summary>
    /// <typeparam name="T">The type of the value associated with the result.</typeparam>
    /// <param name="value">The value to associate with the failed result. This value is not used in the failure case but is required for
    /// extension method syntax.</param>
    /// <param name="fault">The fault information to include in the failed result. Can be null to indicate an unspecified failure.</param>
    /// <returns>A failed result of type <typeparamref name="T"/> containing the specified fault.</returns>
    public static Result<T> ToFailureResult<T>(this T value, Fault? fault)
    {
        return Result<T>.Failure<T>(fault);
    }
    /// <summary>
    /// Creates a failed result with the specified error code and message.
    /// </summary>
    /// <typeparam name="T">The type of the value associated with the result.</typeparam>
    /// <param name="value">The value to associate with the failed result. This value is not used in the failure case but is required for
    /// extension method syntax.</param>
    /// <param name="code">The error code that identifies the reason for failure. Can be null if no code is applicable.</param>
    /// <param name="message">A message describing the failure. Cannot be null.</param>
    /// <returns>A failed result of type T containing the specified error code and message.</returns>
    public static Result<T> ToFailureResult<T>(this T value, string? code, string message)
    {
        return Result<T>.Failure<T>(code, message);
    }
}

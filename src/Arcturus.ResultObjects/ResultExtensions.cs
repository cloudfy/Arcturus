using System.Net;

namespace Arcturus.ResultObjects;

public static class ResultExtensions
{
    /// <summary>
    /// Assigns the <paramref name="exception"/> to the result.
    /// <para>
    /// <b>Result object will be marked as failure.</b>
    /// </para>
    /// </summary>
    /// <typeparam name="T">The type of the result object. Must inherit from Result.</typeparam>
    /// <param name="result">Required.</param>
    /// <param name="exception"></param>
    /// <returns><see cref="Result"/></returns>
    public static T WithException<T>(this T result, Exception exception)
        where T : Result
    {
        result.Exception = exception;
        result.IsSuccess = false;
        return result;
    }
    /// <summary>
    /// Assigns the <paramref name="httpStatusCode"/> to the result.
    /// </summary>
    /// <typeparam name="T">The type of the result object. Must inherit from Result.</typeparam>
    /// <param name="result">Required.</param>
    /// <param name="httpStatusCode"><see cref="HttpStatusCode"/> to assign.</param>
    /// <returns><see cref="Result"/></returns>
    public static T WithHttpStatusCode<T>(this T result, HttpStatusCode httpStatusCode)
         where T : Result
    {
        result.HttpStatusCode = httpStatusCode;
        return result;
    }

    /// <summary>
    /// Assigns a helplink to the result.
    /// </summary>
    /// <typeparam name="T">The type of the result object. Must inherit from Result.</typeparam>
    /// <param name="result">Required.</param>
    /// <param name="uri">Url of the help link.</param>
    /// <returns><see cref="Result"/></returns>
    public static T WithHelpLink<T>(this T result, string uri)
        where T : Result
    {
        result.HelpLink = uri;
        return result;
    }
    /// <summary>
    /// Adds a metadata entry with the specified key and value to the result's metadata bag and returns the result
    /// instance.
    /// </summary>
    /// <remarks>If the metadata bag is not initialized, this method creates it before adding the entry. This
    /// method enables fluent chaining when building or modifying result objects.</remarks>
    /// <typeparam name="T">The type of the result object. Must inherit from Result.</typeparam>
    /// <param name="result">The result instance to which the metadata entry will be added. Cannot be null.</param>
    /// <param name="key">The key for the metadata entry to add. Cannot be null.</param>
    /// <param name="value">The value to associate with the specified key. Can be null.</param>
    /// <returns>The same result instance with the new metadata entry added.</returns>
    public static T WithBag<T>(this T result, string key, object? value)
        where T : Result
    {
        result.Metadata ??= [];
        result.Metadata.Add(key, value);
        return result;
    }
}

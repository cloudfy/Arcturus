namespace Arcturus.Mediation.Abstracts;

/// <summary>
/// Marker interface for requests that do not expect a response.
/// </summary>
public interface IRequest : IAbstractRequest
{
}

/// <summary>
/// Marker interface for requests that expect a response.
/// </summary>
/// <typeparam name="TResponse">The type of response expected.</typeparam>
public interface IRequest<out TResponse>
{
}

public interface IAbstractRequest
{
}

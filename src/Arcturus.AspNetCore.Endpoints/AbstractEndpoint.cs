using Microsoft.AspNetCore.Mvc;

namespace Arcturus.AspNetCore.Endpoints;

/// <summary>
/// Single endpoint abstraction class implemented using the <see cref="EndpointsBuilder"/>.
/// <para>
/// Endpoints will be marked as <see cref="ApiControllerAttribute"/> and will be routed using the <see cref="RouteAttribute"/>. Default [controller] template.
/// </para>
/// </summary>
[ApiController]
[Route("[controller]")]
public abstract class AbstractEndpoint : ControllerBase
{
}
using Microsoft.AspNetCore.Mvc;

namespace Arcturus.AspNetCore.Endpoints;

/// <summary>
/// Single endpoint abstraction class.
/// </summary>
[ApiController]
[Route("[controller]")]
public abstract class AbstractEndpoint : ControllerBase
{
}
﻿using System.Net;

namespace Arcturus.ResultObjects.Specialized;

/// <summary>
/// Provides a fault for a constraint violation (result in HTTP 422).
/// </summary>
/// <param name="Code">Optional code.</param>
/// <param name="Message">A string message indicating the fault.</param>
public record ConstraintFault(string Code, string Message)
    : Fault(Code, Message), ISpecializedFault
{
    public HttpStatusCode HttpStatusCode => HttpStatusCode.UnprocessableContent;
}

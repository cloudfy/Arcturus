﻿namespace Arcturus.EventBus.Abstracts;

public interface IConnection
{
    /// <summary>
    /// Gets the name of the application.
    /// </summary>
    string? ApplicationId { get; }
    /// <summary>
    /// Gets a value indicating if the connection is connected.
    /// </summary>
    bool IsConnected { get; }
}
namespace Arcturus.EventBus.RabbitMQ;

public sealed class RabbitMQEventBusOptions
{
    /// <summary>
    /// Gets or sets an application id.
    /// </summary>
    public string? ApplicationId { get; set; }
    /// <summary>
    /// Gets or sets a client name.
    /// </summary>
    public string? ClientName { get; set; }
    /// <summary>
    /// Gets or sets a host name. Defaults to localhost.
    /// </summary>
    public string? HostName { get; set; }
}
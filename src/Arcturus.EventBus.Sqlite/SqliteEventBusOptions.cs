namespace Arcturus.EventBus.Sqlite;

public class SqliteEventBusOptions
{
    internal string? ApplicationId { get; set; }
    internal string? ClientName { get; set; }
    internal bool UseEventHandlersProcessor { get; set; }

    /// <summary>
    /// Gets or sets a default queue name. Queue name can also be specificed when creating the <see cref="Arcturus.EventBus.Abstracts.IProcessor"/> or <see cref="Arcturus.EventBus.Abstracts.IPublisher"/>, otherwise it will default to 'default_queue'.
    /// </summary>
    public string? DefaultQueueName { get; set; }
    /// <summary>
    /// Gets or sets a connection string to the SQLite database.
    /// <para>
    /// Either set <see cref="ConnectionString" /> or <see cref="DatabasePath" />.
    /// </para>
    /// </summary>
    public string? ConnectionString { get; set; }
    /// <summary>
    /// Gets or sets a path to the SQLite database file.
    /// <para>
    /// Either set <see cref="ConnectionString" /> or <see cref="DatabasePath" />.
    /// </para>
    /// </summary>
    public string? DatabasePath { get; set; }
    /// <summary>
    /// Immediately delete processed events from the database. Defaults to false.
    /// </summary>
    public bool? DeleteProcessedEvents { get; set; }
    /// <summary>
    /// Default interval in milliseconds to process events. Defaults to 100.
    /// </summary>
    public int? ProcessInterval { get; set; }
}

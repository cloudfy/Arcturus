using Arcturus.EventBus.Abstracts;
using Arcturus.EventBus.Serialization;
using Arcturus.EventBus.Sqlite.Internals;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;
using MDS = Microsoft.Data.Sqlite;

namespace Arcturus.EventBus.Sqlite;

/// <summary>
/// Represents a connection to a SQLite database for event bus operations.
/// </summary>
public sealed class SqliteConnection : IConnection
{
    private readonly SqliteEventBusOptions _currentOptions;
    private readonly string _clientName;
    private readonly string? _applicationId;
    private readonly ILogger<SqliteConnection> _logger;
    private readonly string _connectionString;

    private bool _isConnected = false;

    internal SqliteConnection(SqliteEventBusOptions options, ILogger<SqliteConnection> logger)
    {
        _currentOptions = options;
        _clientName = options.ClientName ?? Environment.MachineName;
        _applicationId = options.ApplicationId;
        _logger = logger;

        _connectionString = SqliteConnection.CreateConnectionString(options.ConnectionString, options.DatabasePath);
    }

    internal async Task QueueEvent<TEvent>(TEvent data, string queueName, CancellationToken cancellationToken)
        where TEvent : IEventMessage
    {
        await ReadyDatabase(cancellationToken);

        var eventData = DefaultEventSerializer.Serialize(data);

        var sql = @$"INSERT INTO [Events] (EventId, EventData, QueueName, ClientName) VALUES ('{Guid.NewGuid()}', '{eventData}', '{queueName}', '{_clientName}');";
        using var connection = new MDS.SqliteConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        using var command = new MDS.SqliteCommand(sql, connection);
        await command.ExecuteNonQueryAsync(cancellationToken);
    }
    internal async Task<EventItem[]> PullEvents(
        string queueName, int count = 5, CancellationToken cancellationToken = default)
    {
        await ReadyDatabase(cancellationToken);

        List<EventItem> events = [];

        var sql = $@"
            WITH TopTasks AS (
                SELECT EventId 
                FROM [Events] 
                WHERE QueueName = '{queueName}' AND EventState = 0
                ORDER BY EventSerial DESC 
                LIMIT {count}
            )
            UPDATE [Events] 
            SET EventState = 1 
            WHERE EventId IN (SELECT EventId FROM TopTasks)
            RETURNING EventId, EventData;";
        
        using var connection = new MDS.SqliteConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);
        using var transaction = connection.BeginTransaction();
        try
        {
            using var command = new MDS.SqliteCommand(sql, connection);
            command.Transaction = transaction;
            using var reader = await command.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                var eventId = reader.GetString(0);
                var eventData = reader.GetString(1);
                var @event = DefaultEventSerializer.Deserialize(eventData);
                events.Add(new EventItem(@event, eventId));
            }
            await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while pulling events from SQLite database.");
            await transaction.RollbackAsync(cancellationToken);
        }

        if (_currentOptions.DeleteProcessedEvents.GetValueOrDefault(false))
        {
            var deleteSql = $"DELETE FROM [Events] WHERE EventState = 1 AND QueueName = '{queueName}';";
            using var deleteCommand = new MDS.SqliteCommand(deleteSql, connection);
            await deleteCommand.ExecuteNonQueryAsync(cancellationToken);
        }

        return [.. events];
    }

    private static string CreateConnectionString(string? connectionString, string? databasePath)
    {
        if (string.IsNullOrEmpty(connectionString) &&
            string.IsNullOrEmpty(databasePath))
            throw new ArgumentNullException("Either set connection string or database path.");

        if (string.IsNullOrEmpty(connectionString) == false)
            return connectionString;

        return $"Data Source={databasePath};";
    }

    private static string GetDataSource(string connectionString)
    {
        string pattern = @"Data Source=([^;]+)(?:;|$)";

        Match match = Regex.Match(connectionString, pattern, RegexOptions.IgnoreCase);
        if (match.Success)
        {
            return match.Groups[1].Value;
        }
        throw new ArgumentException("No data source found. Connection string must include 'Data Source='");
    }

    public string? ApplicationId => _applicationId;
    public bool IsConnected => _isConnected;

    private async Task ReadyDatabase(CancellationToken cancellationToken)
    {
        if (_isConnected)
            return;

        // if the datasource is in-memory or already exists, skip initialization
        var datasource = GetDataSource(_connectionString);
        if (datasource == ":memory:" || File.Exists(datasource))
            return;
        
        if (Path.Exists(Path.GetPathRoot(datasource)) == false)
            throw new ArgumentException("Database path must be a valid file path or in-memory. " +
                "If you want to use in-memory, set the connection string to ':memory:'.");

        _logger.LogDebug("Initializing SQLite database at {DataSource}", datasource);

        string[] sql = ["DROP TABLE IF EXISTS Events;", @"CREATE TABLE [Events] (
            EventSerial INTEGER PRIMARY KEY AUTOINCREMENT,
            EventState INTEGER NOT NULL DEFAULT 0,
            EventId TEXT UNIQUE NOT NULL,
            QueueName TEXT NOT NULL,
            EventData TEXT NOT NULL,
            ClientName TEXT NOT NULL DEFAULT 'localhost',
            CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
        );"];

        using var connection = new MDS.SqliteConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        foreach (var sqlStatement in sql)
        {
            using var command = new MDS.SqliteCommand(sqlStatement, connection);
            await command.ExecuteNonQueryAsync(cancellationToken);
        }
        _isConnected = true;
    }
}
// Create and configure the host
using Arcturus.EventBus.Sqlite;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Arcturus.EventBus.Abstracts;

var builder = Host.CreateApplicationBuilder(args);

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Information);

// Register mediation services
builder.Services.AddSqliteEventBus(o => {
    o.ConnectionString = "Data Source=:memory:;";
});

var app = builder.Build();

CancellationTokenSource cts = new ();

var eventBusFactory = app.Services.GetRequiredService<Arcturus.EventBus.Abstracts.IEventBusFactory>();

var eventBusPublisher = eventBusFactory.CreatePublisher();
var processor = eventBusFactory.CreateProcessor();
processor.OnProcessAsync += async (eventMessage, cancellationToken) =>
{
    // Process the event message
    Console.WriteLine($"Processing event: {eventMessage.GetType().Name}");

    if (eventMessage is MyEvent myEvent)
    {
        Console.WriteLine($"Event Name: {myEvent.Name}");
    }
    else
    {
        Console.WriteLine("Unknown event type.");
    }
    await Task.CompletedTask; // Simulate async processing
};
Task.Run(() => processor.WaitForEvents(cts.Token));
Console.WriteLine("Running");

while (!cts.IsCancellationRequested)
{
    Console.WriteLine("Press any key to publish an event or 'q' to quit...");
    var key = Console.ReadKey(true).Key;
    if (key == ConsoleKey.Q)
    {
        cts.Cancel();
        break;
    }

    await eventBusPublisher.Publish(new MyEvent() { Name = "Jack!" });
}


public class MyEvent : IEventMessage
{
    public string Name { get; set; } = "My Event";
}
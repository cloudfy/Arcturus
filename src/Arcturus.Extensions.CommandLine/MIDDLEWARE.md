# Command-Line Middleware Pipeline

This implementation provides an ASP.NET-like middleware pipeline for command-line applications.

## Usage

### 1. Create a Middleware

Implement the `ICommandLineMiddleware` interface:

```csharp
using Arcturus.CommandLine.Abstractions;
using Microsoft.Extensions.Logging;

public class TimingMiddleware : ICommandLineMiddleware
{
    private readonly ILogger<TimingMiddleware> _logger;

    public TimingMiddleware(ILogger<TimingMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(CommandLineContext context, CommandLineDelegate next)
    {
        var sw = Stopwatch.StartNew();
        
        _logger.LogInformation("Command started: {Args}", string.Join(" ", context.Args));
        
        await next(context);
        
        sw.Stop();
        _logger.LogInformation("Command completed in {ElapsedMs}ms", sw.ElapsedMilliseconds);
    }
}
```

### 2. Register Middleware

Use the `UseMiddleware<T>()` extension method on your `IHost`:

```csharp
var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        // Register your commands and handlers
        services.AddCommandLine();
    })
    .Build();

// Register middleware
host.UseMiddleware<LoggingMiddleware>()
    .UseMiddleware<TimingMiddleware>()
    .UseMiddleware<ExceptionHandlingMiddleware>();

// Run the command-line application
await host.RunConsoleCommands<RootCommand>(args);
```

### 3. Access Context in Middleware

The `CommandLineContext` provides access to:

- **Services**: The DI service provider
- **Args**: Command-line arguments
- **Parser**: The System.CommandLine parser
- **CancellationToken**: Cancellation token for the execution
- **RootCommand**: The root command instance
- **Items**: A dictionary for sharing data between middleware

```csharp
public async Task InvokeAsync(CommandLineContext context, CommandLineDelegate next)
{
    // Store data in context
    context.Items["StartTime"] = DateTime.UtcNow;
    
    // Access services
    var myService = context.Services.GetRequiredService<IMyService>();
    
    await next(context);
    
    // Retrieve data from context
    var startTime = (DateTime)context.Items["StartTime"]!;
}
```

## Built-in Middleware

### LoggingMiddleware

Logs command execution timing and exceptions:

```csharp
host.UseMiddleware<LoggingMiddleware>();
```

## Middleware Execution Order

Middleware is executed in the order it is registered. Each middleware can:

1. Execute code **before** the next middleware
2. Call `await next(context)` to invoke the next middleware
3. Execute code **after** the next middleware returns

```
Request Flow:
│
├─> Middleware 1 (before)
│   ├─> Middleware 2 (before)
│   │   ├─> Middleware 3 (before)
│   │   │   └─> Command Execution
│   │   └─> Middleware 3 (after)
│   └─> Middleware 2 (after)
└─> Middleware 1 (after)
```

## Example: Exception Handling Middleware

```csharp
public class ExceptionHandlingMiddleware : ICommandLineMiddleware
{
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(CommandLineContext context, CommandLineDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException ex)
        {
            _logger.LogError("Validation error: {Message}", ex.Message);
            Environment.ExitCode = 1;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred");
            Environment.ExitCode = 1;
        }
    }
}
```

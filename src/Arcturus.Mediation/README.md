# Arcturus.Mediation

This package provides a complete implementation of the Arcturus Mediation framework, enabling CQRS (Command Query Responsibility Segregation) patterns with middleware support and event publishing.

## Features

- **Request/Response Pattern**: Send commands and queries with type-safe responses
- **Middleware Pipeline**: Add cross-cutting concerns like logging, validation, and authorization
- **Event Publishing**: Publish notifications to multiple handlers
- **Dependency Injection**: Built-in support for Microsoft.Extensions.DependencyInjection

## Quick Start

### 1. Register Services

```csharp
services.AddMediation(configuration =>
{
    configuration.RegisterHandlersFromAssembly(typeof(Program).Assembly);
    configuration.AddMiddleware<LoggingMiddleware>();
    configuration.AddMiddleware<ValidationMiddleware>();
});
```

### 2. Define Requests and Handlers

```csharp
// Query with response
public record GetUserQuery(int UserId) : IRequest<User>;

public class GetUserHandler : IRequestHandler<GetUserQuery, User>
{
    public async Task<User> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        // Implementation
        return user;
    }
}

// Command without response
public record DeleteUserCommand(int UserId) : IRequest;

public class DeleteUserHandler : IRequestHandler<DeleteUserCommand>
{
    public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        // Implementation
    }
}
```

### 3. Define Notifications and Handlers

```csharp
public record UserDeletedNotification(int UserId) : INotification;

public class EmailNotificationHandler : INotificationHandler<UserDeletedNotification>
{
    public async Task Handle(UserDeletedNotification notification, CancellationToken cancellationToken)
    {
        // Send email notification
    }
}

public class AuditLogHandler : INotificationHandler<UserDeletedNotification>
{
    public async Task Handle(UserDeletedNotification notification, CancellationToken cancellationToken)
    {
        // Log to audit system
    }
}
```

### 4. Use the Mediator

```csharp
public class UserController : ControllerBase
{
    private readonly IMediator _mediator;

    public UserController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id}")]
    public async Task<User> GetUser(int id)
    {
        return await _mediator.Send(new GetUserQuery(id));
    }

    [HttpDelete("{id}")]
    public async Task DeleteUser(int id)
    {
        await _mediator.Send(new DeleteUserCommand(id));
        await _mediator.Publish(new UserDeletedNotification(id));
    }
}
```

## Middleware

Create custom middleware to add cross-cutting concerns:

```csharp
public class LoggingMiddleware : IMiddleware
{
    private readonly ILogger<LoggingMiddleware> _logger;

    public LoggingMiddleware(ILogger<LoggingMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(IMiddlewareContext context, RequestDelegate next)
    {
        _logger.LogInformation("Handling request {RequestType}", context.RequestType.Name);
        
        var stopwatch = Stopwatch.StartNew();
        await next();
        stopwatch.Stop();
        
        _logger.LogInformation("Handled request {RequestType} in {ElapsedMs}ms", 
            context.RequestType.Name, stopwatch.ElapsedMilliseconds);
    }
}
```

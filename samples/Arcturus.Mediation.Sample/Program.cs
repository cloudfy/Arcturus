using Arcturus.Mediation;
using Arcturus.Mediation.Abstracts;
using Arcturus.Mediation.Middleware;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

// Create and configure the host
var builder = Host.CreateApplicationBuilder(args);

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Information);

// Register mediation services
builder.Services.AddMediation(config =>
{
    config.RegisterHandlersFromAssemblyContaining<Program>();
    config.AddMiddleware<LoggingMiddleware>();
    config.AddMiddleware<PerformanceMiddleware>();
    config.AddMiddleware<ValidationMiddleware>();
});

// Register validators
builder.Services.AddScoped<IValidator<CreateUserCommand>, CreateUserCommandValidator>();

var app = builder.Build();

// Get the mediator
var mediator = app.Services.GetRequiredService<IMediator>();
var logger = app.Services.GetRequiredService<ILogger<Program>>();

logger.LogInformation("=== Arcturus Mediation Demo ===");

try
{
    // Example 1: Query with response
    logger.LogInformation("\n1. Sending GetUserQuery...");
    var user = await mediator.Send(new GetUserQuery(1));
    logger.LogInformation("Retrieved user: {UserName} ({UserEmail})", user.Name, user.Email);

    // Example 2: Command without response
    logger.LogInformation("\n2. Sending CreateUserCommand...");
    await mediator.Send(new CreateUserCommand("Jane Smith", "jane@example.com"));

    // Example 3: Command with response
    logger.LogInformation("\n3. Sending UpdateUserCommand...");
    var updatedUser = await mediator.Send(new UpdateUserCommand(1, "John Updated", "john.updated@example.com"));
    logger.LogInformation("Updated user: {UserName} ({UserEmail})", updatedUser.Name, updatedUser.Email);

    // Example 4: Publishing notifications
    logger.LogInformation("\n4. Publishing UserCreatedNotification...");
    await mediator.Publish(new UserCreatedNotification(2, "Jane Smith", "jane@example.com"));

    // Example 5: Validation failure
    logger.LogInformation("\n5. Testing validation failure...");
    try
    {
        await mediator.Send(new CreateUserCommand("", "invalid-email"));
    }
    catch (ValidationException ex)
    {
        logger.LogWarning("Expected validation error: {Message}", ex.Message);
    }
}
catch (Exception ex)
{
    logger.LogError(ex, "An error occurred during the demo");
}

logger.LogInformation("\n=== Demo Complete ===");

// Domain models
public record User(int Id, string Name, string Email);

// Query with response
public record GetUserQuery(int UserId) : IRequest<User>;

public class GetUserHandler : IRequestHandler<GetUserQuery, User>
{
    private readonly ILogger<GetUserHandler> _logger;

    public GetUserHandler(ILogger<GetUserHandler> logger)
    {
        _logger = logger;
    }

    public async Task<User> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Fetching user {UserId}", request.UserId);
        
        // Simulate async database call
        await Task.Delay(50, cancellationToken);
        
        return new User(request.UserId, "John Doe", "john@example.com");
    }
}

// Command without response
public record CreateUserCommand(string Name, string Email) : IRequest;

public class CreateUserHandler : IRequestHandler<CreateUserCommand>
{
    private readonly ILogger<CreateUserHandler> _logger;

    public CreateUserHandler(ILogger<CreateUserHandler> logger)
    {
        _logger = logger;
    }

    public async Task Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating user: {Name} ({Email})", request.Name, request.Email);
        
        // Simulate async database save
        await Task.Delay(100, cancellationToken);
        
        _logger.LogInformation("User created successfully");
    }
}

// Command with response
public record UpdateUserCommand(int UserId, string Name, string Email) : IRequest<User>;

public class UpdateUserHandler : IRequestHandler<UpdateUserCommand, User>
{
    private readonly ILogger<UpdateUserHandler> _logger;

    public UpdateUserHandler(ILogger<UpdateUserHandler> logger)
    {
        _logger = logger;
    }

    public async Task<User> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating user {UserId}", request.UserId);
        
        // Simulate async database update
        await Task.Delay(75, cancellationToken);
        
        return new User(request.UserId, request.Name, request.Email);
    }
}

// Notification/Event
public record UserCreatedNotification(int UserId, string Name, string Email) : INotification;

public class EmailNotificationHandler : INotificationHandler<UserCreatedNotification>
{
    private readonly ILogger<EmailNotificationHandler> _logger;

    public EmailNotificationHandler(ILogger<EmailNotificationHandler> logger)
    {
        _logger = logger;
    }

    public async Task Handle(UserCreatedNotification notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Sending welcome email to {Email}", notification.Email);
        
        // Simulate email sending
        await Task.Delay(200, cancellationToken);
        
        _logger.LogInformation("Welcome email sent successfully");
    }
}

public class AuditLogHandler : INotificationHandler<UserCreatedNotification>
{
    private readonly ILogger<AuditLogHandler> _logger;

    public AuditLogHandler(ILogger<AuditLogHandler> logger)
    {
        _logger = logger;
    }

    public async Task Handle(UserCreatedNotification notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Logging user creation to audit system: User {UserId}", notification.UserId);
        
        // Simulate audit logging
        await Task.Delay(50, cancellationToken);
        
        _logger.LogInformation("Audit log entry created");
    }
}

// Validator
public class CreateUserCommandValidator : IValidator<CreateUserCommand>
{
    public Task<ValidationResult> ValidateAsync(CreateUserCommand request, CancellationToken cancellationToken = default)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(request.Name))
            errors.Add("Name is required");

        if (string.IsNullOrWhiteSpace(request.Email))
            errors.Add("Email is required");
        else if (!request.Email.Contains('@'))
            errors.Add("Email must be a valid email address");

        var result = errors.Count == 0 
            ? ValidationResult.Success() 
            : ValidationResult.Failure(errors.ToArray());

        return Task.FromResult(result);
    }
}

// Custom middleware
public class PerformanceMiddleware : IMiddleware
{
    private readonly ILogger<PerformanceMiddleware> _logger;

    public PerformanceMiddleware(ILogger<PerformanceMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(IMiddlewareContext context, PipelineRequestDelegate next)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        await next();
        
        stopwatch.Stop();
        
        if (stopwatch.ElapsedMilliseconds > 150)
        {
            _logger.LogWarning("SLOW REQUEST: {RequestType} took {ElapsedMs}ms", 
                context.RequestType.Name, stopwatch.ElapsedMilliseconds);
        }
    }
}

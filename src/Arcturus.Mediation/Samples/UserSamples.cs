using Arcturus.Mediation.Abstracts;

namespace Arcturus.Mediation.Samples;

// Example domain models
public record User(int Id, string Name, string Email);

// Query with response
public record GetUserQuery(int UserId) : IRequest<User>;

public class GetUserHandler : IRequestHandler<GetUserQuery, User>
{
    public async Task<User> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        // Simulate async operation
        await Task.Delay(10, cancellationToken);
        
        // In a real application, this would fetch from a database
        return new User(request.UserId, "John Doe", "john@example.com");
    }
}

// Command without response
public record CreateUserCommand(string Name, string Email) : IRequest;

public class CreateUserHandler : IRequestHandler<CreateUserCommand>
{
    public async Task Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        // Simulate async operation
        await Task.Delay(50, cancellationToken);
        
        // In a real application, this would save to a database
        Console.WriteLine($"Created user: {request.Name} ({request.Email})");
    }
}

// Command with response
public record UpdateUserCommand(int UserId, string Name, string Email) : IRequest<User>;

public class UpdateUserHandler : IRequestHandler<UpdateUserCommand, User>
{
    public async Task<User> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        // Simulate async operation
        await Task.Delay(30, cancellationToken);
        
        // In a real application, this would update the database
        return new User(request.UserId, request.Name, request.Email);
    }
}

// Notification/Event
public record UserCreatedNotification(int UserId, string Name, string Email) : INotification;

public class EmailNotificationHandler : INotificationHandler<UserCreatedNotification>
{
    public async Task Handle(UserCreatedNotification notification, CancellationToken cancellationToken)
    {
        // Simulate sending email
        await Task.Delay(20, cancellationToken);
        Console.WriteLine($"Sent welcome email to {notification.Email}");
    }
}

public class AuditLogHandler : INotificationHandler<UserCreatedNotification>
{
    public async Task Handle(UserCreatedNotification notification, CancellationToken cancellationToken)
    {
        // Simulate audit logging
        await Task.Delay(10, cancellationToken);
        Console.WriteLine($"Audit log: User {notification.UserId} created");
    }
}

public class CacheInvalidationHandler : INotificationHandler<UserCreatedNotification>
{
    public async Task Handle(UserCreatedNotification notification, CancellationToken cancellationToken)
    {
        // Simulate cache invalidation
        await Task.Delay(5, cancellationToken);
        Console.WriteLine($"Invalidated user cache for user {notification.UserId}");
    }
}

using Arcturus.Mediation.Abstracts;
using Arcturus.Mediation.Middleware;

namespace Arcturus.Mediation.Samples;

// Example validator for the CreateUserCommand
public class CreateUserCommandValidator : IValidator<CreateUserCommand>
{
    public Task<ValidationResult> ValidateAsync(CreateUserCommand request, CancellationToken cancellationToken = default)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            errors.Add("Name is required");
        }

        if (string.IsNullOrWhiteSpace(request.Email))
        {
            errors.Add("Email is required");
        }
        else if (!request.Email.Contains('@'))
        {
            errors.Add("Email must be a valid email address");
        }

        var result = errors.Count == 0 
            ? ValidationResult.Success() 
            : ValidationResult.Failure(errors.ToArray());

        return Task.FromResult(result);
    }
}

// Example custom middleware for authorization
public class AuthorizationMiddleware : IMiddleware
{
    public async Task InvokeAsync(IMiddlewareContext context, RequestDelegate next)
    {
        // Simulate authorization check
        var requestType = context.RequestType;
        
        // Example: Only allow certain operations
        if (requestType.Name.Contains("Delete"))
        {
            // In a real app, you'd check user permissions here
            var hasPermission = context.Items.ContainsKey("UserId"); // Simplified check
            
            if (!hasPermission)
            {
                throw new UnauthorizedAccessException($"User not authorized to perform {requestType.Name}");
            }
        }

        await next();
    }
}

// Example performance monitoring middleware
public class PerformanceMiddleware : IMiddleware
{
    public async Task InvokeAsync(IMiddlewareContext context, RequestDelegate next)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        try
        {
            await next();
        }
        finally
        {
            stopwatch.Stop();
            
            // Log slow requests
            if (stopwatch.ElapsedMilliseconds > 1000)
            {
                Console.WriteLine($"SLOW REQUEST: {context.RequestType.Name} took {stopwatch.ElapsedMilliseconds}ms");
            }
            
            // Store metrics
            context.Items["ExecutionTime"] = stopwatch.ElapsedMilliseconds;
        }
    }
}

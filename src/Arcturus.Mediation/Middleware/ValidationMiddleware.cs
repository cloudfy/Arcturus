using Arcturus.Mediation.Abstracts;
using Microsoft.Extensions.Logging;

namespace Arcturus.Mediation.Middleware;

/// <summary>
/// Middleware that validates requests before they are handled.
/// </summary>
public class ValidationMiddleware : IMiddleware
{
    private readonly ILogger<ValidationMiddleware> _logger;
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationMiddleware"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="serviceProvider">The service provider for resolving validators.</param>
    public ValidationMiddleware(ILogger<ValidationMiddleware> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    /// <inheritdoc />
    public async Task InvokeAsync(IMiddlewareContext context, RequestDelegate next)
    {
        var request = context.Request;
        var requestType = context.RequestType;

        // Look for a validator for this request type
        var validatorInterfaceType = typeof(IValidator<>).MakeGenericType(requestType);
        var validator = _serviceProvider.GetService(validatorInterfaceType);

        if (validator != null)
        {
            _logger.LogDebug("Validating request {RequestType}", requestType.Name);

            var validateMethod = validatorInterfaceType.GetMethod("ValidateAsync");
            if (validateMethod != null)
            {
                var result = validateMethod.Invoke(validator, new object[] { request, context.CancellationToken });
                if (result is Task<ValidationResult> validationTask)
                {
                    var validationResult = await validationTask;
                    if (!validationResult.IsValid)
                    {
                        var errors = string.Join(", ", validationResult.Errors);
                        _logger.LogWarning("Validation failed for request {RequestType}: {Errors}", 
                            requestType.Name, errors);
                        
                        throw new ValidationException($"Validation failed for {requestType.Name}: {errors}");
                    }
                }
            }

            _logger.LogDebug("Validation passed for request {RequestType}", requestType.Name);
        }

        await next();
    }
}

/// <summary>
/// Interface for request validators.
/// </summary>
/// <typeparam name="TRequest">The type of request to validate.</typeparam>
public interface IValidator<in TRequest>
{
    /// <summary>
    /// Validates the specified request.
    /// </summary>
    /// <param name="request">The request to validate.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous validation operation.</returns>
    Task<ValidationResult> ValidateAsync(TRequest request, CancellationToken cancellationToken = default);
}

/// <summary>
/// Represents the result of a validation operation.
/// </summary>
public class ValidationResult
{
    /// <summary>
    /// Gets a value indicating whether the validation was successful.
    /// </summary>
    public bool IsValid { get; init; }

    /// <summary>
    /// Gets the validation errors, if any.
    /// </summary>
    public IReadOnlyList<string> Errors { get; init; } = Array.Empty<string>();

    /// <summary>
    /// Creates a successful validation result.
    /// </summary>
    /// <returns>A successful validation result.</returns>
    public static ValidationResult Success() => new() { IsValid = true };

    /// <summary>
    /// Creates a failed validation result with the specified errors.
    /// </summary>
    /// <param name="errors">The validation errors.</param>
    /// <returns>A failed validation result.</returns>
    public static ValidationResult Failure(params string[] errors) => new() 
    { 
        IsValid = false, 
        Errors = errors.ToList().AsReadOnly() 
    };
}

/// <summary>
/// Exception thrown when validation fails.
/// </summary>
public class ValidationException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationException"/> class.
    /// </summary>
    /// <param name="message">The exception message.</param>
    public ValidationException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationException"/> class.
    /// </summary>
    /// <param name="message">The exception message.</param>
    /// <param name="innerException">The inner exception.</param>
    public ValidationException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

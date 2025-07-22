# Arcturus.Mediation.Abstracts

This package provides the core abstractions and interfaces for the Arcturus Mediation framework, enabling CQRS (Command Query Responsibility Segregation) patterns with middleware support and event publishing.

## Core Interfaces

### Request/Response Abstractions
- `IRequest<TResponse>` - Marker interface for requests expecting a response
- `IRequest` - Marker interface for requests with no response
- `IRequestHandler<TRequest, TResponse>` - Handler interface for requests with responses
- `IRequestHandler<TRequest>` - Handler interface for requests without responses

### Event Publishing
- `INotification` - Marker interface for events/notifications
- `INotificationHandler<TNotification>` - Handler interface for events

### Middleware Pipeline
- `IMiddleware` - Interface for middleware components that can wrap request handling

### Mediator
- `IMediator` - Core mediator interface for sending requests and publishing notifications

## Usage

This package contains only the abstractions. For a complete implementation, use the `Arcturus.Mediation` package which provides the concrete mediator implementation and dependency injection extensions.

# Arcturus.EventBus.Abstracts

[![NuGet](https://img.shields.io/nuget/dt/Arcturus.EventBus.Abstracts.svg)](https://www.nuget.org/packages/Arcturus.EventBus.Abstracts) 
[![NuGet](https://img.shields.io/nuget/vpre/Arcturus.EventBus.Abstracts.svg)](https://www.nuget.org/packages/Arcturus.EventBus.Abstracts)

---

Arcturus.EventBus.Abstracts is a .NET library providing a set of abstractions for building event-driven applications. It defines interfaces and base types for publishing, subscribing, and handling integration events, enabling decoupled communication between microservices or components. This package is designed to be used as a foundation for implementing custom or platform-specific event bus solutions.

## Installation

Install the package via NuGet Package Manager or the .NET CLI:

```bash
dotnet add package Arcturus.EventBus.Abstracts
```

Or, using the Package Manager Console:

```powershell
Install-Package Arcturus.EventBus.Abstracts
```

## Prerequisites

- .NET SDK 8 or later

## Features

| Feature                        | Description                                                                                          |
|--------------------------------|------------------------------------------------------------------------------------------------------|
| Event Bus Abstraction          | Provides `IEventBus` interface for publishing and subscribing to events.                             |
| Event Definition               | Defines the `IEvent` interface for strongly-typed event messages.                                    |
| Subscription Management        | Supports event subscription via `EventBusSubscription` for handler registration and management.      |
| Event Handling                 | Abstracts event handling logic with `EventBusHandler` and result types for processing outcomes.      |
| Message Envelope               | Encapsulates event data and metadata using `EventBusMessage`.                                        |
| Configurable Options           | Allows customization of event bus behavior through `EventBusOptions`.                                |
| Error Handling                 | Standardizes error reporting with `EventBusException` and result types.                              |
| Integration Ready              | Designed for easy integration with custom or third-party event bus implementations.                  |
| Extensible Architecture        | Enables extension and adaptation for various messaging platforms and patterns.                       |

## Documentation

For detailed documentation, visit [Arcturus Wiki](https://github.com/cloudfy/Arcturus/wiki).

## License

This project is licensed under the [MIT License](LICENSE) - see the [LICENSE](LICENSE) file for details.

## Support

If you encounter issues or have questions, please file an issue on the [GitHub Issues page](https://github.com/cloudfy/Arcturus/issues).


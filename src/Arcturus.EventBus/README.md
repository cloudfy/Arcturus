# Arcturus.EventBus

[![NuGet](https://img.shields.io/nuget/dt/Arcturus.EventBus.svg)](https://www.nuget.org/packages/Arcturus.EventBus) 
[![NuGet](https://img.shields.io/nuget/vpre/Arcturus.EventBus.svg)](https://www.nuget.org/packages/Arcturus.EventBus)

---

Arcturus.EventBus is a robust .NET library that provides a flexible, extensible event bus abstraction for distributed applications. It enables reliable event-driven communication between microservices and components, supporting multiple transport mechanisms such as Azure Service Bus, Azure Storage Queue, and RabbitMQ. The package simplifies publishing and subscribing to integration events, ensuring decoupled, scalable, and maintainable architectures.

## Installation

Install the package via NuGet Package Manager or the .NET CLI:

```bash
dotnet add package Arcturus.EventBus
```

Or, using the Package Manager Console:

```powershell
Install-Package Arcturus.EventBus
```

## Prerequisites

- .NET SDK 8 or later

## Features

- **Event Bus Abstraction**: Unified interface for publishing and subscribing to events across different message brokers.
- **Integration Event Support**: Strongly-typed event messages for reliable inter-service communication.
- **Multiple Transport Providers**: Built-in support for Azure Service Bus, Azure Storage Queue, and RabbitMQ.
- **Publish/Subscribe Pattern**: Easily publish events and register event handlers for asynchronous processing.
- **Event Handler Registration**: Flexible handler registration for processing events with custom logic.
- **Retry Policies**: Integrated retry mechanisms (via Polly) for resilient event delivery.
- **Serialization**: Automatic event message serialization and deserialization.
- **Diagnostics & Tracing**: OpenTelemetry integration for distributed tracing and diagnostics.
- **Extensibility**: Easily extend with custom transports or event handling strategies.
- **Cancellation & Error Handling**: Support for cancellation tokens and robust error handling during event processing.
- **.NET 8 & .NET 9 Compatibility**: Designed for modern .NET platforms and cloud-native scenarios.

## Documentation

For detailed documentation, visit [Arcturus Wiki](https://github.com/cloudfy/Arcturus/wiki).

## License

This project is licensed under the [MIT License](LICENSE) - see the [LICENSE](LICENSE) file for details.

## Support

If you encounter issues or have questions, please file an issue on the [GitHub Issues page](https://github.com/cloudfy/Arcturus/issues).


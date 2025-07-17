# Arcturus.EventBus.RabbitMQ

[![NuGet](https://img.shields.io/nuget/dt/Arcturus.EventBus.RabbitMQ.svg)](https://www.nuget.org/packages/Arcturus.EventBus.RabbitMQ) 
[![NuGet](https://img.shields.io/nuget/vpre/Arcturus.EventBus.RabbitMQ.svg)](https://www.nuget.org/packages/Arcturus.EventBus.RabbitMQ)

---

Arcturus.EventBus.RabbitMQ is a .NET library that provides a robust RabbitMQ-backed implementation of the Arcturus event bus. It enables distributed applications to reliably publish and subscribe to integration events using RabbitMQ, supporting scalable, decoupled, and resilient messaging patterns.

## Installation

Install the package via NuGet Package Manager or the .NET CLI:

```bash
dotnet add package Arcturus.EventBus.RabbitMQ
```

Or, using the Package Manager Console:

```powershell
Install-Package Arcturus.EventBus.RabbitMQ
```

## Prerequisites

- .NET SDK 8 or later

## Features

- **Publish/Subscribe Messaging**: Send and receive integration events using RabbitMQ queues and exchanges.
- **Reliable Delivery**: Built-in support for message acknowledgement and retry policies (via Polly) to ensure reliable event processing.
- **Consumer and Producer Abstractions**: Easily implement event consumers and producers for your application workflows.
- **Connection Management**: Automatic handling of RabbitMQ connections, including recovery and error handling.
- **Dependency Injection Support**: Seamless integration with Microsoft.Extensions.DependencyInjection for easy registration and configuration.
- **OpenTelemetry Integration**: Optional distributed tracing for event publishing and consumption using OpenTelemetry.
- **Extensible Event Handling**: Flexible event handler registration and custom event serialization.
- **Scalable Architecture**: Designed for high-throughput, distributed systems with support for multiple consumers and producers.
- **Configurable Topology**: Customize exchanges, queues, and routing keys to fit your messaging requirements.

## Documentation

For detailed documentation, visit [Arcturus Wiki](https://github.com/cloudfy/Arcturus/wiki).

## License

This project is licensed under the [MIT License](LICENSE) - see the [LICENSE](LICENSE) file for details.

## Support

If you encounter issues or have questions, please file an issue on the [GitHub Issues page](https://github.com/cloudfy/Arcturus/issues).


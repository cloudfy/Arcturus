# Arcturus.EventBus.OpenTelemetry

[![NuGet](https://img.shields.io/nuget/dt/Arcturus.EventBus.OpenTelemetry.svg)](https://www.nuget.org/packages/Arcturus.EventBus.OpenTelemetry) 
[![NuGet](https://img.shields.io/nuget/vpre/Arcturus.EventBus.OpenTelemetry.svg)](https://www.nuget.org/packages/Arcturus.EventBus.OpenTelemetry)

---

Arcturus.EventBus.OpenTelemetry is a .NET library that provides seamless OpenTelemetry instrumentation for event-driven applications using the Arcturus EventBus. It enables distributed tracing and diagnostics for event publishing and subscription, allowing developers to monitor, analyze, and troubleshoot event flows across microservices and distributed systems.

## Installation

Install the package via NuGet Package Manager or the .NET CLI:

```bash
dotnet add package Arcturus.EventBus.OpenTelemetry
```

Or, using the Package Manager Console:

```powershell
Install-Package Arcturus.EventBus.OpenTelemetry
```

## Prerequisites

- .NET SDK 8 or later

## Features

| Feature                                   | Description                                                                                       |
|--------------------------------------------|---------------------------------------------------------------------------------------------------|
| Distributed Tracing for EventBus           | Automatically creates and propagates OpenTelemetry traces for event publishing and handling.       |
| Activity Source Integration                | Provides an `ActivitySource` for custom event bus activities, enabling fine-grained trace control.|
| Configurable Instrumentation               | Supports options to customize which event bus operations are instrumented and traced.              |
| Diagnostics and Monitoring                 | Enables collection of diagnostics data for event bus operations, improving observability.          |
| Extensible for Custom EventBus Implementations | Designed to work with any Arcturus EventBus implementation, including RabbitMQ and Azure Service Bus. |
| Minimal Overhead                          | Lightweight integration with minimal impact on event bus performance.                             |
| .NET 8 and .NET 9 Support                 | Fully compatible with modern .NET platforms.                                                      |

## Documentation

For detailed documentation, visit [Arcturus Wiki](https://github.com/cloudfy/Arcturus/wiki).

## License

This project is licensed under the [MIT License](LICENSE) - see the [LICENSE](LICENSE) file for details.

## Support

If you encounter issues or have questions, please file an issue on the [GitHub Issues page](https://github.com/cloudfy/Arcturus/issues).


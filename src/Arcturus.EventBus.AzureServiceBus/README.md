# Arcturus.EventBus.AzureServiceBus

[![NuGet](https://img.shields.io/nuget/dt/Arcturus.EventBus.AzureServiceBus.svg)](https://www.nuget.org/packages/Arcturus.EventBus.AzureServiceBus) 
[![NuGet](https://img.shields.io/nuget/vpre/Arcturus.EventBus.AzureServiceBus.svg)](https://www.nuget.org/packages/Arcturus.EventBus.AzureServiceBus)

---

Arcturus.EventBus.AzureServiceBus is a .NET library that provides robust event-driven communication using Azure Service Bus. It enables applications to publish and subscribe to events via queues and topics, supporting scalable, decoupled architectures for distributed systems. The package integrates seamlessly with .NET 8 and .NET 9, leveraging dependency injection and modern cloud messaging patterns.

## Installation

Install the package via NuGet Package Manager or the .NET CLI:

```bash
dotnet add package Arcturus.EventBus.AzureServiceBus
```

Or, using the Package Manager Console:

```powershell
Install-Package Arcturus.EventBus.AzureServiceBus
```

## Prerequisites

- .NET SDK 8 or later

## Features

- **Event Publishing**: Send events to Azure Service Bus queues and topics for reliable, asynchronous processing.
- **Event Subscription**: Receive and handle events from queues and topics, supporting multiple consumers.
- **Queue and Topic Support**: Full integration with Azure Service Bus messaging entities.
- **Dependency Injection**: Easily register event bus services using .NET's DI container.
- **Resilience and Retry Policies**: Built-in support for transient fault handling using Polly.
- **OpenTelemetry Integration**: Optional distributed tracing for event operations.
- **Custom Event Handlers**: Register and manage custom event handlers for different event types.
- **Scalable Architecture**: Designed for high-throughput, cloud-native applications.
- **Extensible Configuration**: Flexible options for customizing connection, retry, and handler behaviors.
- **Error Handling**: Robust error management and logging for failed message deliveries.
- **Documentation and Samples**: Comprehensive documentation and usage examples available.

## Documentation

For detailed documentation, visit [Arcturus Wiki](https://github.com/cloudfy/Arcturus/wiki).

## License

This project is licensed under the [MIT License](LICENSE) - see the [LICENSE](LICENSE) file for details.

## Support

If you encounter issues or have questions, please file an issue on the [GitHub Issues page](https://github.com/cloudfy/Arcturus/issues).


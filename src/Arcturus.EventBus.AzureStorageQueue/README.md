# Arcturus.EventBus.AzureStorageQueue

[![NuGet](https://img.shields.io/nuget/dt/Arcturus.EventBus.AzureStorageQueue.svg)](https://www.nuget.org/packages/Arcturus.EventBus.AzureStorageQueue) 
[![NuGet](https://img.shields.io/nuget/vpre/Arcturus.EventBus.AzureStorageQueue.svg)](https://www.nuget.org/packages/Arcturus.EventBus.AzureStorageQueue)

---

Arcturus.EventBus.AzureStorageQueue is a .NET library that enables event-driven communication using Azure Storage Queues. It allows applications to publish and subscribe to events via Azure queues, supporting scalable, decoupled architectures for distributed systems. The package integrates with .NET 8 and .NET 9, leveraging dependency injection and modern cloud messaging patterns.

## Installation

Install the package via NuGet Package Manager or the .NET CLI:

```bash
dotnet add package Arcturus.EventBus.AzureStorageQueue
```

Or, using the Package Manager Console:

```powershell
Install-Package Arcturus.EventBus.AzureStorageQueue
```

## Prerequisites

- .NET SDK 8 or later

## Features

- **Event Publishing**: Send events to Azure Storage Queues for reliable, asynchronous processing.
- **Event Subscription**: Receive and handle events from Azure Storage Queues, supporting multiple consumers.
- **Dependency Injection Support**: Easily register event bus services using .NET's DI container.
- **Resilience and Retry Policies**: Built-in support for transient fault handling and retries using Polly.
- **OpenTelemetry Integration**: Optional distributed tracing for event operations.
- **Custom Event Handlers**: Register and manage custom event handlers for different event types.
- **Dead-letter Handling**: Support for managing failed messages and dead-letter scenarios.
- **Message Serialization**: Automatic serialization and deserialization of event messages.
- **Scalable Architecture**: Designed for high-throughput, cloud-native applications.
- **Extensible Configuration**: Flexible options for customizing connection, retry, and handler behaviors.
- **Error Handling and Logging**: Robust error management and logging for failed message deliveries.

## Documentation

For detailed documentation, visit [Arcturus Wiki](https://github.com/cloudfy/Arcturus/wiki).

## License

This project is licensed under the [MIT License](LICENSE) - see the [LICENSE](LICENSE) file for details.

## Support

If you encounter issues or have questions, please file an issue on the [GitHub Issues page](https://github.com/cloudfy/Arcturus/issues).


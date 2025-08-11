# Arcturus.EventBus.Sqlite

[![NuGet](https://img.shields.io/nuget/dt/Arcturus.EventBus.Sqlite.svg)](https://www.nuget.org/packages/Arcturus.EventBus.Sqlite) 
[![NuGet](https://img.shields.io/nuget/vpre/Arcturus.EventBus.Sqlite.svg)](https://www.nuget.org/packages/Arcturus.EventBus.Sqlite)

---

Arcturus.EventBus.Sqlite is a .NET library that provides a lightweight, file-based event bus implementation using SQLite. It enables distributed applications to reliably publish and subscribe to integration events using a local or remote SQLite database, supporting scalable, decoupled, and resilient messaging patterns for development and production scenarios.

## Installation

Install the package via NuGet Package Manager or the .NET CLI:

```bash
dotnet add package Arcturus.EventBus.Sqlite
```

Or, using the Package Manager Console:

```powershell
Install-Package Arcturus.EventBus.Sqlite
```

## Prerequisites

- .NET SDK 8 or later

## Features

- **Event Publishing**: Send events to a SQLite database for reliable, asynchronous processing.
- **Event Subscription**: Receive and handle events from SQLite, supporting multiple consumers.
- **Dependency Injection Support**: Easily register event bus services using .NET's DI container.
- **Configurable Storage**: Use either a connection string or database file path for SQLite storage.
- **Resilience and Retry Policies**: Built-in support for transient fault handling and retries.
- **Custom Event Handlers**: Register and manage custom event handlers for different event types.
- **Scalable Architecture**: Designed for high-throughput, distributed systems.
- **Extensible Configuration**: Flexible options for customizing connection, event processing interval, and handler behaviors.
- **Error Handling and Logging**: Robust error management and logging for failed message deliveries.
- **OpenTelemetry Integration**: Optional distributed tracing for event operations.
- **.NET 8 and .NET 9 Support**: Fully compatible with modern .NET platforms.

## Documentation

For detailed documentation, visit [Arcturus Wiki](https://github.com/cloudfy/Arcturus/wiki).

## License

This project is licensed under the [MIT License](LICENSE) - see the [LICENSE](LICENSE) file for details.

## Support

If you encounter issues or have questions, please file an issue on the [GitHub Issues page](https://github.com/cloudfy/Arcturus/issues).


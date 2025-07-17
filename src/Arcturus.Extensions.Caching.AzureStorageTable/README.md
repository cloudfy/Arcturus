# Arcturus.Extensions.Caching.AzureStorageTable

[![NuGet](https://img.shields.io/nuget/dt/Arcturus.Extensions.Caching.AzureStorageTable.svg)](https://www.nuget.org/packages/Arcturus.Extensions.Caching.AzureStorageTable) 
[![NuGet](https://img.shields.io/nuget/vpre/Arcturus.Extensions.Caching.AzureStorageTable.svg)](https://www.nuget.org/packages/Arcturus.Extensions.Caching.AzureStorageTable)

---

Arcturus.Extensions.Caching.AzureStorageTable is a .NET library that provides distributed caching capabilities using Azure Storage Table as the backend. It enables applications to store, retrieve, and manage cache entries in Azure Table Storage, making it suitable for scalable cloud-native solutions that require persistent and shared cache across multiple instances.

## Installation

Install the package via NuGet Package Manager or the .NET CLI:

```bash
dotnet add package Arcturus.Extensions.Caching.AzureStorageTable
```

Or, using the Package Manager Console:

```powershell
Install-Package Arcturus.Extensions.Caching.AzureStorageTable
```

## Prerequisites

- .NET SDK 8 or later

## Features

- Distributed caching using Azure Table Storage as the backend.
- Implements Microsoft.Extensions.Caching.Abstractions for seamless integration.
- Supports storing and retrieving complex objects as cache entries.
- Configurable cache expiration and eviction policies.
- Thread-safe operations for concurrent access.
- Easy setup with dependency injection and configuration.
- Scalable for cloud-native and multi-instance deployments.
- Minimal dependencies for lightweight integration.

## Documentation

For detailed documentation, visit [Arcturus Wiki](https://github.com/cloudfy/Arcturus/wiki).

## License

This project is licensed under the [MIT License](LICENSE) - see the [LICENSE](LICENSE) file for details.

## Support

If you encounter issues or have questions, please file an issue on the [GitHub Issues page](https://github.com/cloudfy/Arcturus/issues).


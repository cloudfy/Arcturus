# Arcturus.Extensions.Configuration.AzureStorageBlob

[![NuGet](https://img.shields.io/nuget/dt/Arcturus.Extensions.Configuration.AzureStorageBlob.svg)](https://www.nuget.org/packages/Arcturus.Extensions.Configuration.AzureStorageBlob) 
[![NuGet](https://img.shields.io/nuget/vpre/Arcturus.Extensions.Configuration.AzureStorageBlob.svg)](https://www.nuget.org/packages/Arcturus.Extensions.Configuration.AzureStorageBlob)

---

Arcturus.Extensions.Configuration.AzureStorageBlob is a .NET library that enables applications to load configuration settings directly from Azure Storage Blob containers. This package integrates with the Microsoft.Extensions.Configuration system, allowing you to centralize and manage your configuration files in Azure, supporting dynamic updates and secure storage for distributed/cloud-native applications.

## Installation

Install the package via NuGet Package Manager or the .NET CLI:

```bash
dotnet add package Arcturus.Extensions.Configuration.AzureStorageBlob
```

Or, using the Package Manager Console:

```powershell
Install-Package Arcturus.Extensions.Configuration.AzureStorageBlob
```

## Prerequisites

- .NET SDK 8 or later

## Features

- Load configuration from Azure Storage Blob containers
- Supports JSON, XML, and other text-based formats
- Optional local fallback if blob is unavailable
- Automatic reload on blob changes (if enabled)
- Secure access via connection string, SAS token, or Managed Identity
- Seamless integration with Microsoft.Extensions.Configuration
- Suitable for distributed and cloud-native .NET applications

## Documentation

For detailed documentation, visit [Arcturus Wiki](https://github.com/cloudfy/Arcturus/wiki).

## License

This project is licensed under the [MIT License](LICENSE) - see the [LICENSE](LICENSE) file for details.

## Support

If you encounter issues or have questions, please file an issue on the [GitHub Issues page](https://github.com/cloudfy/Arcturus/issues).


# Arcturus
Arcturus is a ready-to-use framework designed for building modern cloud and distributed applications. It streamlines the development process by offering robust tools and integrations, enabling developers to create scalable, efficient, and resilient systems with ease.

## Give a Star! :star:

If you like or are using this project to learn or start your solution, please give it a star. Thanks!

## Packages
Arcturus consist of multiple independant packages. Some are extensions of other packages, most are individual packages.

### Result Object
* [Arcturus.ResultObjects](https://github.com/cloudfy/Arcturus/wiki/ResultObjects): Enable control of application state via `Result` rather than exceptions.
* [Arcturus.ResultObjects for AspNetCore](https://github.com/cloudfy/Arcturus/wiki/ResultObjects-(AspNetCore-extension)): An extension to `Arcturus.ResultObjects` which enables return of normalized HTTP status codes using `ProblemDetails`.

### Caching
* [Arcturus.Extensions.Caching](https://github.com/cloudfy/Arcturus/wiki/Caching): Provides common extensions for IDistributedCache interfaces.
* [Arcturus.Extensions.Caching.AzureStorageTable](https://github.com/cloudfy/Arcturus/wiki/Caching): Distributed cache implementation using Azure Storage Tables.

### Data & Repository
* [Arcturus.Repository.Abstracts](https://github.com/cloudfy/Arcturus/wiki/Repository): Provide abstract data models for using the repository implementations.
* [Arcturus.Repository.EntityFrameworkCore](https://github.com/cloudfy/Arcturus/wiki/Repository): Repository implementation using Entity Framework Core using SQL server.
* [Arcturus.Repository.EntityFrameworkCore.PostgresSql](https://github.com/cloudfy/Arcturus/wiki/Repository): Repository implementation using Entity Framework Core using Postgres SQL.
* [Arcturus.Extensions.Repository.Pagination](https://github.com/cloudfy/Arcturus/wiki/Repository#pagination): Extensions supporting pagination response using [Arcturus.ResultObjects](https://github.com/cloudfy/Arcturus/wiki/ResultObjects).

### Patchable
* [Arcturus.Patchable](https://github.com/cloudfy/Arcturus/wiki/Patchable): Enable Patch endpoints to partial updates.
* [Arcturus.Extensions.Patchable.AspNetCore](https://github.com/cloudfy/Arcturus/wiki/Patchable): ASP.NET adoptation of patch endpoints.
  
### ASP.Net Core
* [Arcturus.AspNetCore.Endpoints](https://github.com/cloudfy/Arcturus/wiki/Endpoints-(AspNetCore)): Provides endpoint builder pattern to setup MVC based controller endpoints.

### EventBus
* [Arcturus.EventBus](https://github.com/cloudfy/Arcturus/wiki/EventBus): Provides eventbus implementation.
* [Arcturus.EventBus.Abstracts](https://github.com/cloudfy/Arcturus/wiki/EventBus): Provides abstracts eventbus implementation.
* [Arcturus.EventBus.RabbitMQ](https://github.com/cloudfy/Arcturus/wiki/EventBus): RabbitMQ implementation of the event bus.
* [Arcturus.EventBus.AzureStorageQueue](https://github.com/cloudfy/Arcturus/wiki/EventBs): Azure Storage Queue implementation of the event bus.
* [Arcturus.EventBus.AzureServiceBus](https://github.com/cloudfy/Arcturus/wiki/EventBus): Azure Service Bus implementatino of the event bus.

### CommandLine
* [Arcturus.Extensions.CommandLine](): An extension to [System.CommandLine](https://learn.microsoft.com/en-us/dotnet/standard/commandline/) which enables dependency injection and command handler implementation.

### Configuration
* [Arcturus.Extensions.Configuration.AzureStorageBlob](); Enable storing configuration options in Azure Blob Storage.

## Read the Wiki
As Arcturus consist of multiple packages, we are keeping documentation in the [Wiki](https://github.com/cloudfy/Arcturus/wiki).

## How can I contribute?
We welcome contributions! Many people all over the world have helped make .NET better.

Follow instructions in [contributing.md](CONTRIBUTING.md) for working in the code in the repository.

## License
The code in this repo is licensed under the [MIT](LICENSE) license.

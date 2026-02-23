# Arcturus.DevHost.Hosting

The **Arcturus.DevHost.Hosting** library provides a lightweight orchestration framework for running multiple .NET projects and executables together in a local development environment. It simplifies the developer inner loop by managing the lifecycle of multiple services, applications, and processes from a single orchestrator.

## Overview

This library enables you to:

- **Orchestrate multiple .NET projects**: Launch and manage multiple .NET applications simultaneously
- **Run arbitrary executables**: Start and monitor external processes like npm, docker, or any other command-line tools
- **Centralized logging**: Configure and view logs from all managed resources in one place
- **Process lifecycle management**: Automatic process cleanup and graceful shutdown handling
- **Environment configuration**: Set custom URLs and environment variables per resource

## Key Components

### OrchestratorHost

The main host that manages the lifecycle of all registered resources (projects and executables). It handles:
- Starting and stopping processes
- Signal handling (CTRL+C, process termination)
- Job object management for process cleanup
- Coordinating logging across all resources

### OrchestratorHostBuilder

A fluent builder API for configuring your orchestrator:

```csharp
var host = OrchestratorHostBuilder.Create()
    .AddProject<MyApiProject>(project => 
    {
        project.WithUrls("http://localhost:5000");
        project.WithEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
    })
    .AddExecutable("npm", "run dev", workingDirectory: "./frontend")
    .ConfigureLogging(logging => 
    {
        logging.AddConsole();
    })
    .Build();

await host.RunAsync();
```

### Resources

- **ProjectResource**: Represents a .NET project with metadata, URLs, and environment variables
- **ExecutableResource**: Represents an external executable with arguments and working directory
- **IResource**: Common interface for all managed resources

## Usage

### 1. Define Project Metadata

Implement `IProjectMetadata` for each project you want to orchestrate:

```csharp
public class MyApiProjectMetadata : IProjectMetadata
{
    public string ProjectPath => "path/to/MyApi/MyApi.csproj";
    public string ProjectName => "MyApi";
}
```

### 2. Build and Run the Orchestrator

```csharp
using Arcturus.DevHost.Hosting;

var host = OrchestratorHostBuilder.Create()
    .AddProject<MyApiProjectMetadata>()
    .AddProject<MyWebAppMetadata>(project =>
    {
        project.WithUrls("http://localhost:3000");
    })
    .AddExecutable("npm", "run dev", "./client-app")
    .ConfigureLogging(logging =>
    {
        logging.AddConsole();
        logging.SetMinimumLevel(LogLevel.Information);
    })
    .Build();

await host.RunAsync();
```

### 3. Graceful Shutdown

The orchestrator automatically handles shutdown signals and ensures all child processes are terminated cleanly.

## Features

- **Job Object Management**: Uses Windows Job Objects to ensure child processes are terminated when the orchestrator exits
- **Cancellation Support**: Integrates with CancellationToken for graceful shutdown
- **Extensible Logging**: Supports any Microsoft.Extensions.Logging provider
- **Fluent Configuration**: Clean and intuitive builder pattern

## Use Cases

- Running a web API alongside a frontend development server
- Starting multiple microservices for local testing
- Orchestrating background workers, databases, and APIs together
- Simplifying complex multi-project development environments

## Related Projects

- **Arcturus.DevHost.Sdk**: Core SDK types and abstractions
- **Arcturus.DevHost.SourceGenerator**: Code generation for project metadata
- **Arcturus.DevHost.OrchestratorTemplate**: Project template for creating orchestrators
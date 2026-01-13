# Arcturus.CodeAnalysis.CSharp

A Roslyn analyzer package that provides code analysis rules to help enforce best practices and prevent common pitfalls in C# development.

## Features

This analyzer package currently includes the following diagnostic rules:

- **SDA001: Avoid direct HttpClient instantiation** - Detects direct instantiation of `HttpClient` which can lead to socket exhaustion. Recommends using `IHttpClientFactory` or a shared instance instead.

## Installation

Install the package via NuGet Package Manager:

```bash
dotnet add package Arcturus.CodeAnalysis.CSharp
```

Or via Package Manager Console:

```powershell
Install-Package Arcturus.CodeAnalysis.CSharp
```

Or add it directly to your `.csproj` file:

```xml
<ItemGroup>
  <PackageReference Include="Arcturus.CodeAnalysis.CSharp" Version="1.0.0" />
</ItemGroup>
```

## Usage

Once installed, the analyzers will automatically run during compilation and provide warnings or errors for detected issues in your code.

### Example: HttpClient Instantiation

**Bad Practice (will trigger SDA001 warning):**

```csharp
public class MyService
{
    public async Task<string> GetDataAsync()
    {
        var client = new HttpClient(); // Warning: Avoid direct HttpClient instantiation
        return await client.GetStringAsync("https://api.example.com");
    }
}
```

**Recommended Approach:**

```csharp
public class MyService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public MyService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<string> GetDataAsync()
    {
        var client = _httpClientFactory.CreateClient();
        return await client.GetStringAsync("https://api.example.com");
    }
}
```

## Diagnostic Rules

| Rule ID | Category | Severity | Description |
|---------|----------|----------|-------------|
| SDA001  | Usage    | Warning  | Avoid direct HttpClient instantiation to prevent socket exhaustion |

## Configuration

You can configure the behavior of individual analyzers using an `.editorconfig` file in your project:

```ini
# Disable SDA001
dotnet_diagnostic.SDA001.severity = none

# Make SDA001 an error instead of warning
dotnet_diagnostic.SDA001.severity = error

# Keep as warning (default)
dotnet_diagnostic.SDA001.severity = warning
```

## Requirements

- .NET Standard 2.0 or higher
- Compatible with .NET Framework 4.7.2+, .NET Core 2.0+, and .NET 5.0+

## Contributing

Contributions are welcome! If you have suggestions for new analyzer rules or improvements to existing ones, please open an issue or submit a pull request.

## License

This project is part of the Arcturus framework. Please refer to the main repository for license information.

# Arcturus.DevHost.SourceGenerator

A C# source generator that automatically generates strongly-typed project metadata classes from a list of project references.

## Overview

This source generator reads a `ProjectReferences.txt` file during compilation and generates C# classes that implement `IProjectMetadata` for each project reference. This enables type-safe access to project metadata at compile-time and runtime, which is particularly useful for development orchestration scenarios.

## What It Does

The generator performs the following operations:

1. **Reads Project References**: Parses a `ProjectReferences.txt` file that contains tab-delimited project information
2. **Generates Classes**: Creates a class for each project that implements `Arcturus.DevHost.Hosting.Abstracts.IProjectMetadata`
3. **Provides Metadata**: Each generated class exposes:
   - `ProjectPath`: The full path to the project file
   - `ProjectName`: A sanitized, valid C# identifier name for the project

## Input Format

The generator expects a `ProjectReferences.txt` file with the following tab-delimited format:

```
<column1>\t<ProjectDisplayName>\t<ProjectFilePath>
```

- **Column 1**: Reserved/unused
- **Column 2**: The display name of the project (will be sanitized to create a valid C# class name)
- **Column 3**: The full path to the project file

## Generated Code

For each line in `ProjectReferences.txt`, the generator creates a class like:

```csharp
public class ProjectDisplayName : global::Arcturus.DevHost.Hosting.Abstracts.IProjectMetadata
{
    public string ProjectPath => "C:\\Path\\To\\Project.csproj";
    public string ProjectName => "ProjectDisplayName";
}
```

All generated classes are placed in the `Projects` namespace and output to `Projects.g.cs`.

## Name Sanitization

Project names are sanitized to create valid C# identifiers:
- Spaces, dots, and hyphens are replaced with underscores
- Names starting with digits are prefixed with an underscore

## Versioning

The generator respects the following MSBuild properties for versioning (in order of precedence):
1. `Version`
2. `AssemblyVersion`
3. `PackageVersion`
4. Falls back to "1.0.0" if none are specified

## Usage

1. Add the `Arcturus.DevHost.SourceGenerator` NuGet package to your project
2. Create a `ProjectReferences.txt` file in your project
3. Set the build action for the file to `AdditionalFiles`
4. Build your project - the generator will automatically create the `Projects.g.cs` file

## Requirements

- .NET Standard 2.0 or higher
- Roslyn 4.8.0 or compatible version

## Integration

This generator is designed to work with the Arcturus DevHost ecosystem, particularly for orchestrating development environments where you need programmatic access to project metadata.
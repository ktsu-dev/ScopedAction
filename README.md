# ktsu.ScopedAction

> A lightweight utility for executing paired actions at the start and end of code blocks.

[![License](https://img.shields.io/github/license/ktsu-dev/ScopedAction)](https://github.com/ktsu-dev/ScopedAction/blob/main/LICENSE.md)
[![NuGet](https://img.shields.io/nuget/v/ktsu.ScopedAction.svg)](https://www.nuget.org/packages/ktsu.ScopedAction/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/ktsu.ScopedAction.svg)](https://www.nuget.org/packages/ktsu.ScopedAction/)
[![Build Status](https://github.com/ktsu-dev/ScopedAction/workflows/build/badge.svg)](https://github.com/ktsu-dev/ScopedAction/actions)
[![GitHub Stars](https://img.shields.io/github/stars/ktsu-dev/ScopedAction?style=social)](https://github.com/ktsu-dev/ScopedAction/stargazers)

## Introduction

`ktsu.ScopedAction` is a .NET utility that provides a clean way to execute actions at the beginning and end of code blocks. It leverages C#'s `using` statement and the `IDisposable` pattern to ensure that paired operations (like resource acquisition/release, state changes, or logging) are properly executed, even in the presence of exceptions.

## Features

- **Paired Actions**: Execute actions when entering and exiting a scope
- **Exception Safety**: Cleanup actions execute even if exceptions occur
- **Lightweight**: Simple API with minimal overhead
- **Flexible**: Works with any action delegates
- **Extendable**: Can be subclassed for specialized behaviors
- **Resource Management**: Follows .NET's standard disposal pattern

## Installation

### Package Manager Console

```powershell
Install-Package ktsu.ScopedAction
```

### .NET CLI

```bash
dotnet add package ktsu.ScopedAction
```

### Package Reference

```xml
<PackageReference Include="ktsu.ScopedAction" Version="x.y.z" />
```

## Usage Examples

### Basic Example

```csharp
using ktsu.ScopedAction;

// Execute actions at the beginning and end of a scope
using (new ScopedAction(
    onOpen: () => Console.WriteLine("Entering the scope"),
    onClose: () => Console.WriteLine("Exiting the scope")))
{
    // Any code here...
    Console.WriteLine("Inside the scope");
}

// Output:
// Entering the scope
// Inside the scope
// Exiting the scope
```

### Resource Management

```csharp
// Manage resources with paired acquisition and release
public void ProcessFile(string filePath)
{
    using (new ScopedAction(
        onOpen: () => Console.WriteLine($"Opening file: {filePath}"),
        onClose: () => Console.WriteLine($"Closing file: {filePath}")))
    {
        // Process the file...
        Console.WriteLine("Processing file contents");
    }
}
```

### Measuring Performance

```csharp
// Measure and log execution time of operations
public void TimedOperation()
{
    var stopwatch = new Stopwatch();
    
    using (new ScopedAction(
        onOpen: () => stopwatch.Start(),
        onClose: () => {
            stopwatch.Stop();
            Console.WriteLine($"Operation completed in {stopwatch.ElapsedMilliseconds}ms");
        }))
    {
        // Operation to be timed
        PerformComplexCalculation();
    }
}
```

## Advanced Usage

### Temporary State Changes

```csharp
// Temporarily change application state
private bool _isProcessing;

public void ProcessWithState()
{
    using (new ScopedAction(
        onOpen: () => _isProcessing = true,
        onClose: () => _isProcessing = false))
    {
        // Code that requires _isProcessing to be true
        PerformProcessing();
    }
    
    // _isProcessing is automatically reset to false here
}
```

### Creating Custom ScopedAction Classes

```csharp
// Create specialized scoped actions by extending the base class
public class LoggingScope : ScopedAction
{
    private readonly string _operationName;
    private readonly ILogger _logger;
    
    public LoggingScope(string operationName, ILogger logger)
    {
        _operationName = operationName;
        _logger = logger;
        
        _logger.LogInformation($"Starting operation: {_operationName}");
    }
    
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _logger.LogInformation($"Completed operation: {_operationName}");
        }
        
        base.Dispose(disposing);
    }
}

// Usage
using (new LoggingScope("Data Import", logger))
{
    // Import data...
}
```

## API Reference

### ScopedAction Class

The primary class for executing actions at scope boundaries.

#### Constructors

| Constructor | Parameters | Description |
|-------------|------------|-------------|
| `ScopedAction(Action? onOpen, Action? onClose)` | `onOpen`: Action executed on construction<br>`onClose`: Action executed on disposal | Creates a new ScopedAction that executes the specified actions |
| `ScopedAction()` | None | Protected constructor for derived classes |

#### Methods

| Method | Return Type | Description |
|--------|-------------|-------------|
| `Dispose()` | `void` | Executes the onClose action if not already disposed |
| `Dispose(bool disposing)` | `void` | Protected virtual method for implementing the dispose pattern |

## Contributing

Contributions are welcome! Here's how you can help:

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

Please make sure to update tests as appropriate.

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details.

# ktsu.ScopedAction

> A lightweight utility for executing paired actions at the start and end of code blocks.

[![License](https://img.shields.io/github/license/ktsu-dev/ScopedAction)](https://github.com/ktsu-dev/ScopedAction/blob/main/LICENSE.md)
[![NuGet](https://img.shields.io/nuget/v/ktsu.ScopedAction.svg)](https://www.nuget.org/packages/ktsu.ScopedAction/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/ktsu.ScopedAction.svg)](https://www.nuget.org/packages/ktsu.ScopedAction/)
[![Build Status](https://github.com/ktsu-dev/ScopedAction/workflows/build/badge.svg)](https://github.com/ktsu-dev/ScopedAction/actions)
[![GitHub Stars](https://img.shields.io/github/stars/ktsu-dev/ScopedAction?style=social)](https://github.com/ktsu-dev/ScopedAction/stargazers)

## Introduction

`ktsu.ScopedAction` is a .NET utility that provides an abstract base class for executing actions at the beginning and end of code blocks. It implements the RAII (Resource Acquisition Is Initialization) pattern and leverages C#'s `using` statement and the `IDisposable` pattern to ensure that paired operations (like resource acquisition/release, state changes, or logging) are properly executed, even in the presence of exceptions.

As an abstract class, `ScopedAction` is designed to be inherited and extended to create specialized scoped behavior classes tailored to specific use cases.

## Features

- **Abstract Base Class**: Provides a foundation for creating specialized scoped action classes
- **RAII Pattern**: Implements Resource Acquisition Is Initialization for deterministic resource management
- **Paired Actions**: Execute actions when entering and exiting a scope
- **Exception Safety**: Cleanup actions execute even if exceptions occur
- **Lightweight**: Simple API with minimal overhead
- **Inheritance-Based**: Designed to be extended for domain-specific implementations
- **Flexible**: Works with any action delegates through protected constructor
- **Resource Management**: Follows .NET's standard disposal pattern

## RAII (Resource Acquisition Is Initialization)

`ktsu.ScopedAction` implements the RAII pattern, a programming idiom that binds the life cycle of a resource to the lifetime of an object. This ensures that:

- **Automatic Resource Management**: Resources are automatically acquired when the object is constructed and released when it's destroyed
- **Exception Safety**: Resources are properly released even if exceptions occur within the scope
- **Deterministic Cleanup**: The cleanup action is guaranteed to execute when the object goes out of scope
- **Stack-Based Semantics**: Leverages C#'s `using` statement to provide stack-based resource management similar to C++ RAII

The pattern is particularly useful for scenarios like:
- File operations (open/close)
- Database transactions (begin/commit or rollback)
- Lock management (acquire/release)
- Performance timing (start/stop)
- Temporary state changes (set/restore)

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

## Usage Example

```csharp
using ktsu.ScopedAction;

// Create a simple derived class for logging
public class LoggingScope : ScopedAction
{
    public LoggingScope(string operation)
        : base(onOpen: () => Enter(operation), onClose: () => Exit(operation))
    {
    }

    private static void Enter(string operation) => Console.WriteLine($"Entering: {operation}");
    private static void Exit(string operation) => Console.WriteLine($"Exiting: {operation}");
}

// Usage
using (new LoggingScope("my operation"))
{
    // Any code here...
    Console.WriteLine("Inside the scope");
}

// Output:
// Entering: my operation
// Inside the scope
// Exiting: my operation
```

## API Reference

### ScopedAction Class

An abstract base class for executing actions at scope boundaries. This class must be inherited to create concrete implementations.

#### Constructors

| Constructor | Parameters | Description |
|-------------|------------|-------------|
| `ScopedAction(Action? onOpen, Action? onClose)` | `onOpen`: Action executed on construction<br>`onClose`: Action executed on disposal | Protected constructor for derived classes that executes the specified actions |
| `ScopedAction()` | None | Private parameterless constructor |

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

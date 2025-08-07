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
- **Deterministic Execution**: The OnClose action is guaranteed to execute when the object goes out of scope
- **Stack-Based Semantics**: Leverages C#'s `using` statement to provide execution tied to lexical scope, mimicking C++ stack-based object destruction

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

## Usage Examples

The `ScopedAction` class supports three main patterns, progressing from simple to more complex scenarios:

### Example 1: Using static methods without parameters

```csharp
using ktsu.ScopedAction;

public class ConsoleMarkerScope() : ScopedAction(Enter, Exit)
{
    // Using method groups - no lambdas needed when methods match Action signature
    private static void Enter() => Console.WriteLine("Entering scope");
    private static void Exit() => Console.WriteLine("Exiting scope");
}

// Usage
using (new ConsoleMarkerScope())
{
    // Any code here...
    Console.WriteLine("Inside the scope");
}

// Output:
// Entering scope
// Inside the scope
// Exiting scope
```

### Example 2: Using static methods with parameters

```csharp
using ktsu.ScopedAction;

public class LoggingScope(string operation)
    : ScopedAction(() => Enter(operation), () => Exit(operation))
{
    // Using lambdas to capture constructor parameters for static methods
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

### Example 3: Using instance members

```csharp
using ktsu.ScopedAction;

// This approach enables access to instance members in the OnClose action
public class TimingScope : ScopedAction
{
    private readonly DateTime startTime;  // Instance field
    private readonly string operation;    // Instance field

    public TimingScope(string operation)
    {
        this.operation = operation;
        this.startTime = DateTime.Now;

        // OnClose can reference instance method that accesses instance members
        OnClose = LogExecutionTime;

        // No need to assign an OnOpen action - it would execute immediately anyway.
        // Instead, just perform the "on open" logic directly in the constructor.
        Console.WriteLine($"Starting: {operation}");
    }

    // Instance method with access to instance fields
    private void LogExecutionTime()
    {
        // Can directly access instance members: startTime, operation
        var elapsed = DateTime.Now - startTime;
        Console.WriteLine($"Completed: {operation} in {elapsed.TotalMilliseconds:F2}ms");
    }
}

// Usage
using (new TimingScope("database query"))
{
    // Simulate some work
    Thread.Sleep(100);
    Console.WriteLine("Executing query...");
}

// Output:
// Starting: database query
// Executing query...
// Completed: database query in 100.xx ms
```

#### Choosing the Right Pattern

**Example 1 (Method Groups)**: Use when you have simple static methods with no parameters. This is the most concise approach.

**Example 2 (Lambda Capture)**: Use when you need to pass constructor parameters to static methods. Lambdas capture the parameters from the constructor scope.

**Example 3 (Instance Members)**: Use when your OnClose logic needs access to **instance state** (fields, properties, methods). This pattern is essential for:
- Complex resource management
- Stateful cleanup operations  
- Scenarios where disposal behavior depends on data initialized during construction

The parameterless constructor approach gives you full access to the object's state, while the action-based constructors are limited to static methods and captured parameters.

## API Reference

### ScopedAction Class

An abstract base class for executing actions at scope boundaries. This class must be inherited to create concrete implementations.

#### Constructors

| Constructor | Parameters | Description |
|-------------|------------|-------------|
| `ScopedAction(Action? onOpen, Action? onClose)` | `onOpen`: Action executed on construction<br>`onClose`: Action executed on disposal | Protected constructor for derived classes that executes the onOpen action immediately and stores the onClose action for later execution during disposal |
| `ScopedAction()` | None | Protected parameterless constructor for derived classes that need custom initialization |

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| `OnClose` | `Action?` | Protected property that stores the action to execute when the scoped action is disposed. Can be set by derived classes. |

#### Methods

| Method | Return Type | Description |
|--------|-------------|-------------|
| `Dispose()` | `void` | Public method that implements the IDisposable interface. Executes the OnClose action if not already disposed and suppresses finalization. |
| `Dispose(bool disposing)` | `void` | Protected virtual method for implementing the standard .NET dispose pattern. Executes the OnClose action when disposing is true and handles multiple disposal calls safely. |

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

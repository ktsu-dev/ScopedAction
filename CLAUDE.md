# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

ScopedAction is a .NET library that provides an abstract base class implementing the RAII (Resource Acquisition Is Initialization) pattern. It executes paired actions at the beginning and end of code blocks using C#'s `using` statement and `IDisposable` pattern.

## Build Commands

```bash
# Build the library
dotnet build

# Build with Release configuration
dotnet build --configuration Release

# Run all tests
dotnet test

# Run a specific test by name
dotnet test --filter "FullyQualifiedName~ScopedActionTests.ActionExecutionOrder_IsCorrect"

# Create NuGet package
dotnet pack --configuration Release --output ./staging
```

## Architecture

### Core Class: `ScopedAction`

The library consists of a single abstract class (`ScopedAction/ScopedAction.cs`) that:

- Implements `IDisposable` with the standard dispose pattern
- Provides two protected constructors:
  - `ScopedAction(Action? onOpen, Action? onClose)` - Executes `onOpen` immediately, stores `onClose` for disposal
  - `ScopedAction()` - Parameterless for custom initialization patterns
- Exposes `OnClose` as a protected settable property for derived classes
- Ensures `OnClose` executes only once via `disposedValue` flag

### Inheritance Patterns

Derived classes use three patterns (see README.md for examples):

1. **Method Groups**: Static methods without parameters passed directly to base constructor
2. **Lambda Capture**: Lambdas capturing constructor parameters for static methods
3. **Instance Members**: Parameterless constructor with `OnClose` property assignment for stateful cleanup

## Project Structure

- `ScopedAction/` - Main library (multi-targeted: net10.0, net9.0, net8.0, net7.0, net6.0, net5.0, netstandard2.1, netstandard2.0)
- `ScopedAction.Tests/` - MSTest tests (targets net10.0 only)
- Uses `ktsu.Sdk` for standardized build configuration
- Uses Central Package Management via `Directory.Packages.props`

## Testing

Tests use MSTest.Sdk with Microsoft.Testing.Platform runner. Key test scenarios:

- OnOpen executes immediately on construction
- OnClose executes on disposal (only once)
- Null actions are handled gracefully
- Exception safety (cleanup runs even with exceptions)
- Execution order verification

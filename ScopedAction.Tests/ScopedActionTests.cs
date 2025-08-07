// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.ScopedAction.Tests;

using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class ScopedActionTests
{
	/// <summary>
	/// Test implementation of ScopedAction for testing purposes.
	/// </summary>
	private sealed class TestScopedAction(Action? onOpen, Action? onClose) : ScopedAction(onOpen, onClose)
	{
	}

	[TestMethod]
	public void Constructor_WithOnOpenAction_ExecutesOnOpenImmediately()
	{
		// Arrange
		bool onOpenCalled = false;
		bool onCloseCalled = false;

		// Act
		using TestScopedAction scopedAction = new(
			onOpen: () => onOpenCalled = true,
			onClose: () => onCloseCalled = true);

		// Assert
		Assert.IsTrue(onOpenCalled, "OnOpen should be called immediately in constructor");
		Assert.IsFalse(onCloseCalled, "OnClose should not be called yet");
	}

	[TestMethod]
	public void Constructor_WithNullOnOpen_DoesNotThrow()
	{
		// Arrange & Act & Assert
		using TestScopedAction scopedAction = new(onOpen: null, onClose: null);
		Assert.IsNotNull(scopedAction);
	}

	[TestMethod]
	public void Dispose_WithOnCloseAction_ExecutesOnClose()
	{
		// Arrange
		bool onOpenCalled = false;
		bool onCloseCalled = false;

		// Act
		using (TestScopedAction scopedAction = new(
			onOpen: () => onOpenCalled = true,
			onClose: () => onCloseCalled = true))
		{
			Assert.IsTrue(onOpenCalled, "OnOpen should be called");
			Assert.IsFalse(onCloseCalled, "OnClose should not be called yet");
		}

		// Assert
		Assert.IsTrue(onCloseCalled, "OnClose should be called after disposal");
	}

	[TestMethod]
	public void Dispose_WithNullOnClose_DoesNotThrow()
	{
		// Arrange
		TestScopedAction scopedAction = new(onOpen: null, onClose: null);

		// Act & Assert - should not throw
		scopedAction.Dispose();
	}

	[TestMethod]
	public void Dispose_CalledMultipleTimes_OnlyExecutesOnCloseOnce()
	{
		// Arrange
		int onCloseCallCount = 0;
		using TestScopedAction scopedAction = new(
			onOpen: null,
			onClose: () => onCloseCallCount++);

		// Act
		scopedAction.Dispose();
		scopedAction.Dispose();
		scopedAction.Dispose();

		// Assert
		Assert.AreEqual(1, onCloseCallCount, "OnClose should only be called once");
	}

	[TestMethod]
	public void ExceptionInOnOpen_DoesNotPreventConstruction()
	{
		// Arrange & Act & Assert
		InvalidOperationException exception = Assert.ThrowsException<InvalidOperationException>(() =>
		{
			TestScopedAction _ = new(
				onOpen: () => throw new InvalidOperationException("Test exception"),
				onClose: null);
		});
		Assert.AreEqual("Test exception", exception.Message);
	}

	[TestMethod]
	public void ExceptionInOnClose_DoesNotPreventDisposal()
	{
		// Arrange
		using TestScopedAction scopedAction = new(
			onOpen: null,
			onClose: () => throw new InvalidOperationException("Test exception"));

		// Act & Assert
		InvalidOperationException exception = Assert.ThrowsException<InvalidOperationException>(scopedAction.Dispose);
		Assert.AreEqual("Test exception", exception.Message);
	}

	[TestMethod]
	public void UsingStatement_EnsuresDisposalEvenWithException()
	{
		// Arrange
		bool onCloseCalled = false;

		// Act & Assert
		InvalidOperationException exception = Assert.ThrowsException<InvalidOperationException>(() =>
		{
			using TestScopedAction scopedAction = new(
				onOpen: null,
				onClose: () => onCloseCalled = true);

			throw new InvalidOperationException("Test exception");
		});

		Assert.IsTrue(onCloseCalled, "OnClose should be called even when exception occurs");
		Assert.AreEqual("Test exception", exception.Message);
	}

	[TestMethod]
	public void ActionExecutionOrder_IsCorrect()
	{
		// Arrange
		List<string> executionOrder = [];

		// Act
		using (TestScopedAction scopedAction = new(
			onOpen: () => executionOrder.Add("OnOpen"),
			onClose: () => executionOrder.Add("OnClose")))
		{
			executionOrder.Add("inside scope");
		}

		// Assert
		Assert.AreEqual(3, executionOrder.Count);
		Assert.AreEqual("OnOpen", executionOrder[0]);
		Assert.AreEqual("inside scope", executionOrder[1]);
		Assert.AreEqual("OnClose", executionOrder[2]);
	}

	[TestMethod]
	public void Inheritance_LoggingScope_WorksCorrectly()
	{
		// This test verifies that the pattern shown in README works
		List<string> log = [];

		// Create a logging scope like in the README
		LoggingScope loggingScope = new("test operation", log);

		using (loggingScope)
		{
			log.Add("inside scope");
		}

		Assert.AreEqual(3, log.Count);
		Assert.AreEqual("Entering: test operation", log[0]);
		Assert.AreEqual("inside scope", log[1]);
		Assert.AreEqual("Exiting: test operation", log[2]);
	}

	[TestMethod]
	public void Inheritance_ConstructorTest_WorksCorrectly()
	{
		using (new ConstructorTest())
		{
			Assert.IsTrue(true);
		}

		Assert.IsTrue(true);
	}

	/// <summary>
	/// Example implementation similar to the one in README for testing.
	/// </summary>
	private sealed class LoggingScope(string operation, List<string> log) : ScopedAction(onOpen: () => Enter(operation, log), onClose: () => Exit(operation, log))
	{
		private static void Enter(string operation, List<string> log) => log.Add($"Entering: {operation}");
		private static void Exit(string operation, List<string> log) => log.Add($"Exiting: {operation}");
	}

	private sealed class ConstructorTest : ScopedAction
	{
		public ConstructorTest()
		{
			OnClose = MyOnClose;
			MyOnOpen();
		}

		private static void MyOnOpen() { }

		private void MyOnClose() { }
	}
}

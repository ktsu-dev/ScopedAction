// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.ScopedAction;

/// <summary>
/// A disposable class that executes actions at the beginning and end of a scope.
/// </summary>
public abstract class ScopedAction : IDisposable
{
	private int disposed;

	/// <summary>
	/// The action to execute when the scoped action is disposed.
	/// </summary>
	protected Action? OnClose { get; set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="ScopedAction"/> class.
	/// </summary>
	/// <param name="onOpen">The action to execute when the scoped action is created.</param>
	/// <param name="onClose">The action to execute when the scoped action is disposed.</param>
	protected ScopedAction(Action? onOpen, Action? onClose)
	{
		OnClose = onClose;
		onOpen?.Invoke();
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="ScopedAction"/> class.
	/// </summary>
	protected ScopedAction() { }

	/// <summary>
	/// Dispose of the <see cref="ScopedAction"/>.
	/// </summary>
	/// <param name="disposing"></param>
	protected virtual void Dispose(bool disposing)
	{
		// Claim disposal before running OnClose, so a Dispose that arrives while OnClose is still running,
		// from inside OnClose or from another thread, finds the instance already disposed
		if (Interlocked.Exchange(ref disposed, 1) != 0)
		{
			return;
		}

		if (disposing)
		{
			OnClose?.Invoke();
		}
	}

	/// <summary>
	/// Dispose of the <see cref="ScopedAction"/>.
	/// </summary>
	public void Dispose()
	{
		// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
}

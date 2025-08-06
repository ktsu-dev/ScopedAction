// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.ScopedAction;

/// <summary>
/// A disposable class that executes actions at the beginning and end of a scope.
/// </summary>
public abstract class ScopedAction : IDisposable
{
	private bool disposedValue;
	internal readonly Action? onOpen;
	internal readonly Action? onClose;

	/// <summary>
	/// Initializes a new instance of the <see cref="ScopedAction"/> class.
	/// </summary>
	/// <param name="onOpen">The action to execute when the scoped action is created.</param>
	/// <param name="onClose">The action to execute when the scoped action is disposed.</param>
	protected ScopedAction(Action? onOpen, Action? onClose)
	{
		this.onOpen = onOpen;
		this.onClose = onClose;
		onOpen?.Invoke();
	}

	private ScopedAction() { }

	/// <summary>
	/// Dispose of the <see cref="ScopedAction"/>.
	/// </summary>
	/// <param name="disposing"></param>
	protected virtual void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			if (disposing)
			{
				try
				{
					onClose?.Invoke();
				}
				finally
				{
					disposedValue = true;
				}
			}
			else
			{
				disposedValue = true;
			}
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

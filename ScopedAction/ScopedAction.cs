#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace ktsu.ScopedAction;

public class ScopedAction : IDisposable
{
	private bool disposedValue;
	protected Action? OnOpen { get; init; }
	protected Action? OnClose { get; init; }
	public ScopedAction(Action? onOpen, Action? onClose)
	{
		OnOpen = onOpen;
		OnClose = onClose;
		onOpen?.Invoke();
	}

	protected ScopedAction() { }

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
				OnClose?.Invoke();
			}

			disposedValue = true;
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

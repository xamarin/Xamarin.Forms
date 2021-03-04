using System;

namespace Microsoft.Maui.Handlers.Benchmarks
{
	public class WindowStub : IWindow
	{
		public IMauiContext MauiContext { get; set; }
		public IPage Content { get; set; }

		public void Close()
		{
			Closed?.Invoke(this, EventArgs.Empty);
		}

		public void OnCreated()
		{
			Created?.Invoke(this, EventArgs.Empty);
		}

		public void OnPaused()
		{
			Paused?.Invoke(this, EventArgs.Empty);
		}

		public void OnResumed()
		{
			Resumed?.Invoke(this, EventArgs.Empty);
		}

		public void OnStopped()
		{
			Stopped?.Invoke(this, EventArgs.Empty);
		}

		public event EventHandler Closed;
		public event EventHandler Created;
		public event EventHandler Resumed;
		public event EventHandler Paused;
		public event EventHandler Stopped;
	}
}
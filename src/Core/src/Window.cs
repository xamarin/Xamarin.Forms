using System;

namespace Microsoft.Maui
{
	public class Window : IWindow
	{
		public Window()
		{
			OnCreated();
		}

		public IPage? Content { get; set; }

		public IMauiContext? MauiContext { get; set; }

		public event EventHandler? Closed;

		public event EventHandler? Created;

		public event EventHandler? Resumed;

		public event EventHandler? Paused;

		public event EventHandler? Stopped;

		public virtual void OnCreated()
		{
			Created?.Invoke(this, EventArgs.Empty);
		}

		public virtual void OnResumed()
		{
			Resumed?.Invoke(this, EventArgs.Empty);
		}

		public virtual void OnPaused()
		{
			Paused?.Invoke(this, EventArgs.Empty);
		}

		public virtual void OnStopped()
		{
			Stopped?.Invoke(this, EventArgs.Empty);
			Closed?.Invoke(this, EventArgs.Empty);
		}
	}
}
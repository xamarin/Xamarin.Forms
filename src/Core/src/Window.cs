using System;

namespace Microsoft.Maui
{
	public class Window : IWindow, IDisposable
	{
		public Window()
		{
			Initialize();
		}

		public IPage? Content { get; set; }

		public IMauiContext? MauiContext { get; set; }

		public bool IsActive { get; set; }

		public event EventHandler? Closed;

		public event EventHandler? Created;

		public event EventHandler? Resumed;

		public event EventHandler? Paused;

		public event EventHandler? Stopped;

		public void Dispose()
		{
			Content = null;
		}

		public void Show()
		{
		
		}

		public void Activate()
		{
			
		}

		public void Hide()
		{
			
		}

		public void Close()
		{
			Dispose();
			OnStopped();
		}

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

		void Initialize()
		{
			if (Application.Current != null)
			{
				Application.Current.Windows.Add(this);

				if (Application.Current.MainWindow == null)
				{
					Application.Current.MainWindow = this;
				}
			}

			OnCreated();
		}	
	}
}
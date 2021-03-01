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
		}

		public virtual void OnCreated()
		{

		}

		public virtual void OnResumed()
		{

		}

		public virtual void OnPaused()
		{
		
		}

		public virtual void OnStopped()
		{
		
		}

		void Initialize()
		{
			if (App.Current != null)
			{
				App.Current.Windows.Add(this);

				if (App.Current.MainWindow == null)
				{
					App.Current.MainWindow = this;
				}
			}
		}	
	}
}
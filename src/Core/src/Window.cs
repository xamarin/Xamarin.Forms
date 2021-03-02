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
			OnStopped();
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
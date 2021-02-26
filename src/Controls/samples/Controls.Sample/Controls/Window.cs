using Microsoft.Maui;

namespace Maui.Controls.Sample.Controls
{
	public class Window : IWindow
	{
		public IPage Page { get; set; }
		public IMauiContext MauiContext { get; set; }

		public virtual void Create()
		{
		}

		public virtual void Resume()
		{
		}

		public virtual void Pause()
		{
		}

		public virtual void Stop()
		{
		}
	}
}
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui;
using System.Diagnostics;
using Application = Microsoft.Maui.Application;

namespace Maui.Controls.Sample
{
	public class App : Application
	{
		public override IWindow CreateWindow(IActivationState state)
		{
			return Services.GetService<IWindow>();
		}
				
		public override void OnCreated()
		{
			Debug.WriteLine("Application Created.");
		}

		public override void OnPaused()
		{
			Debug.WriteLine("Application Paused.");
		}

		public override void OnResumed()
		{
			Debug.WriteLine("Application Resumed.");
		}

		public override void OnStopped()
		{
			Debug.WriteLine("Application Stopped.");
		}
	}
}
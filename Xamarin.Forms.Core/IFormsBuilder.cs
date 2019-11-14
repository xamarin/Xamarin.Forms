using System;

namespace Xamarin.Forms
{
	public interface IFormsBuilder
	{
		Application Build(Type app);
		TApp Build<TApp>() where TApp : Application;
		TApp Build<TApp>(Func<TApp> createApp) where TApp : Application;
		IFormsBuilder UseStartup(Type startupType);
		IFormsBuilder UseStartup<TStartup>() where TStartup : IStartup;
		IFormsBuilder UseStartup<TStartup>(Func<TStartup> createStartup) where TStartup : IStartup;
	}
}
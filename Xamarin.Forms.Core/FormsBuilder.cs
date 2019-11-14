#if NETSTANDARD2_0
using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Xamarin.Forms.Internals;
#endif

namespace Xamarin.Forms
{
	class FormsBuilder : IFormsBuilder
	{
		IStartup _startup;

		public Application Build(Type app)
		{
			BuildHost(app);
			Application formsApp = null;
#if NETSTANDARD2_0
			formsApp = Application.ServiceProvider?.GetService(app) as Application;
#endif
			return formsApp ?? Activator.CreateInstance(app) as Application;
		}

		public TApp Build<TApp>() where TApp : Application
		{
			BuildHost(typeof(TApp));
			TApp formsApp = null;
#if NETSTANDARD2_0
			formsApp = Application.ServiceProvider?.GetService<TApp>();
#endif
			return formsApp ?? Activator.CreateInstance<TApp>();
		}

		public TApp Build<TApp>(Func<TApp> createApp) where TApp : Application
		{
			BuildHost(typeof(TApp));
			return createApp();
		}

		public IFormsBuilder UseStartup(Type startupType)
		{
			_startup = Activator.CreateInstance(startupType) as IStartup;
			return this;
		}

		public IFormsBuilder UseStartup<TStartup>() where TStartup : IStartup
		{
			_startup = Activator.CreateInstance<TStartup>();
			return this;
		}

		public IFormsBuilder UseStartup<TStartup>(Func<TStartup> createStartup) where TStartup : IStartup
		{
			_startup = createStartup();
			return this;
		}

		void BuildHost(Type app)
		{
#if NETSTANDARD2_0
			EmbeddedResourceLoader.SetExecutingAssembly(app.Assembly);
			IHost host = new HostBuilder().ConfigureHostConfiguration(c =>
			{
				c.AddCommandLine(new[] { $"ContentRoot={Environment.CurrentDirectory}" });
				c.AddJsonStream(EmbeddedResourceLoader.GetEmbeddedResourceStream("appsettings.json"));
				_startup?.ConfigureHostConfiguration(c);
			}).ConfigureServices((h, s) =>
			{
				//nativeConfigureServices(c, x);
				//ConfigureServices(c, x);
				_startup?.ConfigureServices(h, s);
			}).ConfigureLogging(l => l.AddConsole(o =>
			{
				o.DisableColors = true;
			})).Build();

			Application.ServiceProvider = host.Services;
#endif
		}
	}
}
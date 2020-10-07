using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualBasic;
using NUnit.Framework;
using Xamarin.Forms;
using Xamarin.Platform.Hosting;

namespace Xamarin.Platform.Handlers.Tests
{
	[TestFixture]
	public partial class HostBuilderTests
	{

		[Test]
		public void CanBuildAHost()
		{
			var host = App.CreateDefaultBuilder().Build();
			Assert.IsNotNull(host);
		}

		[Test]
		public void CanGetApp()
		{
			var (host, app) = App.CreateDefaultBuilder()
							  .Init<MockApp>();
			Assert.IsNotNull(app);
			Assert.IsInstanceOf(typeof(MockApp), app);
		}

		[Test]
		public void CanGetStaticApp()
		{
			var (host, app) = App.CreateDefaultBuilder()
						  .Init<MockApp>();

			Assert.IsNotNull(App.Current);
			Assert.AreEqual(App.Current, app);
		}

		[Test]
		public void CanGetServices()
		{
			var (host, app) = App.CreateDefaultBuilder()
							  .Init<MockApp>();

			Assert.IsNotNull(app.Services);
		}

		[Test]
		public void CanGetStaticServices()
		{
			var (host, app) = App.CreateDefaultBuilder()
							  .Init<MockApp>();

			Assert.IsNotNull(App.Current.Services);
			Assert.AreEqual(app.Services, App.Current.Services);
		}

		[Test]
		public void CanRegisterAndGetHandler()
		{
			var builder = App.CreateDefaultBuilder()
							   .RegisterHandler<IMockView, MockViewHandler>();
			var (host, app) = (builder as IAppHostBuilder).Init<MockApp>();

			var handler = App.Current.Handlers.GetHandler(typeof(IMockView));
			Assert.IsNotNull(handler);
			Assert.IsInstanceOf(typeof(MockViewHandler), handler);
		}

		[Test]
		public void CanRegisterAndGetHandlerWithDictionary()
		{
			var builder = App.CreateDefaultBuilder()
							.RegisterHandlers(new Dictionary<Type, Type> {
								{ typeof(IMockView), typeof(MockViewHandler) }
							});
			var (host, app) = (builder as IAppHostBuilder).Init<MockApp>();

			var handler = App.Current.Handlers.GetHandler(typeof(IMockView));
			Assert.IsNotNull(handler);
			Assert.IsInstanceOf(typeof(MockViewHandler), handler);
		}

		[Test]
		public void CanRegisterAndGetHandlerForType()
		{
			var builder = App.CreateDefaultBuilder()
							.RegisterHandler<IMockView, MockViewHandler>();

			var (host, app) = (builder as IAppHostBuilder).Init<MockApp>();

			var handler = App.Current.Handlers.GetHandler(typeof(MockView));
			Assert.IsNotNull(handler);
			Assert.IsInstanceOf(typeof(MockViewHandler), handler);
		}

		[Test]
		public void DefaultHandlersAreRegistered()
		{
			var (host, app) = App.CreateDefaultBuilder()
							.Init<MockApp>();

			var handler = App.Current.Handlers.GetHandler(typeof(IButton));
			Assert.IsNotNull(handler);
			Assert.IsInstanceOf(typeof(ButtonHandler), handler);
		}

		[Test]
		public void CanSpecifyHandler()
		{
			var builder = App.CreateDefaultBuilder()
							.RegisterHandler<MockButton, MockButtonHandler>();

			var (host, app) = (builder as IAppHostBuilder).Init<MockApp>();

			var defaultHandler = App.Current.Handlers.GetHandler(typeof(IButton));
			var specificHandler = App.Current.Handlers.GetHandler(typeof(MockButton));
			Assert.IsNotNull(defaultHandler);
			Assert.IsNotNull(specificHandler);
			Assert.IsInstanceOf(typeof(ButtonHandler), defaultHandler);
			Assert.IsInstanceOf(typeof(MockButtonHandler), specificHandler);
		}

		[Test]
		public void Get10000Handlers()
		{
			int iterations = 10000;
			var (host, app) = App.CreateDefaultBuilder()
						 .Init<MockApp>();

			var handlerWarmup = app.Handlers.GetHandler<Button>();
			;
			Stopwatch watch = Stopwatch.StartNew();
			for (int i = 0; i < iterations; i++)
			{
				var defaultHandler = app.Handlers.GetHandler<Button>();
				Assert.NotNull(defaultHandler);
			}
			watch.Stop();
			var total = watch.ElapsedMilliseconds;
			watch.Reset();
			Registrar.Handlers.Register<IButton, ButtonHandler>();
			watch.Start();
			for (int i = 0; i < iterations; i++)
			{
				var defaultHandler = Registrar.Handlers.GetHandler<Button>();
				Assert.NotNull(defaultHandler);
			}
			watch.Stop();
			var totalRegistrar = watch.ElapsedMilliseconds;
			Console.WriteLine($"Elapsed time DI: {total} and Registrar: {totalRegistrar}");
		}

		AppBuilder _builder;

		[Test]
		public void Register100Handlers()
		{
			int iterations = 10000;
			_builder = new AppBuilder();
			for (int i = 0; i < iterations; i++)
			{
				_builder.RegisterHandler<IButton, ButtonHandler>();
			}
			var host = _builder.Build();

		}

		[Test]
		public async Task StartSTopHost()
		{
			var (host, app) = App.CreateDefaultBuilder().Init<MockApp>();

			host.Start();

			await host.StopAsync();
		}
	}
}

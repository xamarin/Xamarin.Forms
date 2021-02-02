using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualBasic;
using Xunit;
using Xamarin.Forms;
using Xamarin.Platform.Hosting;
using Xamarin.Platform.Handlers.Tests;

namespace Xamarin.Platform.Handlers.UnitTests
{
	[Category(TestCategory.Core, TestCategory.Hosting)]
	public partial class HostBuilderTests
	{

		[Fact]
		public void CanBuildAHost()
		{
			var host = App.CreateDefaultBuilder().Build();
			Assert.NotNull(host);
		}

		[Fact]
		public void CanGetApp()
		{
			var (host, app) = App.CreateDefaultBuilder()
							  .Init<MockApp>();
			Assert.NotNull(app);
			Assert.IsType<MockApp>(app);
		}

		[Fact]
		public void CanGetStaticApp()
		{
			var (host, app) = App.CreateDefaultBuilder()
						  .Init<MockApp>();

			Assert.NotNull(App.Current);
			Assert.Equal(App.Current, app);
		}

		[Fact]
		public void CanGetServices()
		{
			var (host, app) = App.CreateDefaultBuilder()
							  .Init<MockApp>();

			Assert.NotNull(app.Services);
		}

		[Fact]
		public void CanGetStaticServices()
		{
			var (host, app) = App.CreateDefaultBuilder()
							  .Init<MockApp>();

			Assert.NotNull(App.Current.Services);
			Assert.Equal(app.Services, App.Current.Services);
		}

		[Fact]
		public void CanRegisterAndGetHandler()
		{
			var builder = App.CreateDefaultBuilder()
							   .RegisterHandler<IMockView, MockViewHandler>();
			var (host, app) = (builder as IAppHostBuilder).Init<MockApp>();

			var handler = App.Current.Handlers.GetHandler(typeof(IMockView));
			Assert.NotNull(handler);
			Assert.IsType<MockViewHandler>(handler);
		}

		[Fact]
		public void CanRegisterAndGetHandlerWithDictionary()
		{
			var builder = App.CreateDefaultBuilder()
							.RegisterHandlers(new Dictionary<Type, Type> {
								{ typeof(IMockView), typeof(MockViewHandler) }
							});
			var (host, app) = (builder as IAppHostBuilder).Init<MockApp>();

			var handler = App.Current.Handlers.GetHandler(typeof(IMockView));
			Assert.NotNull(handler);
			Assert.IsType<MockViewHandler>(handler);
		}

		[Fact]
		public void CanRegisterAndGetHandlerForType()
		{
			var builder = App.CreateDefaultBuilder()
							.RegisterHandler<IMockView, MockViewHandler>();

			var (host, app) = (builder as IAppHostBuilder).Init<MockApp>();

			var handler = App.Current.Handlers.GetHandler(typeof(MockView));
			Assert.NotNull(handler);
			Assert.IsType<MockViewHandler>(handler);
		}

		[Fact]
		public void DefaultHandlersAreRegistered()
		{
			var (host, app) = App.CreateDefaultBuilder()
							.Init<MockApp>();

			var handler = App.Current.Handlers.GetHandler(typeof(IButton));
			Assert.NotNull(handler);
			Assert.IsType<ButtonHandler>(handler);
		}

		[Fact]
		public void CanSpecifyHandler()
		{
			var builder = App.CreateDefaultBuilder()
							.RegisterHandler<MockButton, MockButtonHandler>();

			var (host, app) = (builder as IAppHostBuilder).Init<MockApp>();

			var defaultHandler = App.Current.Handlers.GetHandler(typeof(IButton));
			var specificHandler = App.Current.Handlers.GetHandler(typeof(MockButton));
			Assert.NotNull(defaultHandler);
			Assert.NotNull(specificHandler);
			Assert.IsType<ButtonHandler>(defaultHandler);
			Assert.IsType<MockButtonHandler>( specificHandler);
		}

		[Fact]
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

		[Fact]
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

		[Fact]
		public async Task StartSTopHost()
		{
			var (host, app) = App.CreateDefaultBuilder().Init<MockApp>();

			host.Start();

			await host.StopAsync();
		}
	}
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
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
			var app = App.CreateDefaultBuilder()
							  .Build<AppStub>();
			Assert.NotNull(app);
			Assert.IsType<AppStub>(app);
		}

		[Fact]
		public void CanGetStaticApp()
		{
			var app = App.CreateDefaultBuilder()
						  .Build<AppStub>();

			Assert.NotNull(App.Current);
			Assert.Equal(App.Current, app);
		}

		[Fact]
		public void CanGetServices()
		{
			var app = App.CreateDefaultBuilder()
							  .Build<AppStub>();

			Assert.NotNull(app.Services);
		}

		[Fact]
		public void CanGetStaticServices()
		{
			var app = App.CreateDefaultBuilder()
							  .Build<AppStub>();

			Assert.NotNull(App.Current.Services);
			Assert.Equal(app.Services, App.Current.Services);
		}

		[Fact]
		public void CanRegisterAndGetHandler()
		{
			var app = App.CreateDefaultBuilder()
							   .RegisterHandler<IViewStub, ViewHandlerStub>()
							   .Build<AppStub>();

			var handler = App.Current.Handlers.GetHandler(typeof(IViewStub));
			Assert.NotNull(handler);
			Assert.IsType<ViewHandlerStub>(handler);
		}

		[Fact]
		public void CanRegisterAndGetHandlerWithDictionary()
		{
			var app = App.CreateDefaultBuilder()
							.RegisterHandlers(new Dictionary<Type, Type> {
								{ typeof(IViewStub), typeof(ViewHandlerStub) }
							})
							.Build<AppStub>();

			var handler = App.Current.Handlers.GetHandler(typeof(IViewStub));
			Assert.NotNull(handler);
			Assert.IsType<ViewHandlerStub>(handler);
		}

		[Fact]
		public void CanRegisterAndGetHandlerForType()
		{
			var app = App.CreateDefaultBuilder()
							.RegisterHandler<IViewStub, ViewHandlerStub>()
							.Build<AppStub>();

			var handler = App.Current.Handlers.GetHandler(typeof(ViewStub));
			Assert.NotNull(handler);
			Assert.IsType<ViewHandlerStub>(handler);
		}

		[Fact]
		public void DefaultHandlersAreRegistered()
		{
			var app = App.CreateDefaultBuilder()
							.Build<AppStub>();

			var handler = App.Current.Handlers.GetHandler(typeof(IButton));
			Assert.NotNull(handler);
			Assert.IsType<ButtonHandler>(handler);
		}

		[Fact]
		public void CanSpecifyHandler()
		{
			var app = App.CreateDefaultBuilder()
							.RegisterHandler<ButtonStub, ButtonHandlerStub>()
							.Build<AppStub>();

			var defaultHandler = App.Current.Handlers.GetHandler(typeof(IButton));
			var specificHandler = App.Current.Handlers.GetHandler(typeof(ButtonStub));
			Assert.NotNull(defaultHandler);
			Assert.NotNull(specificHandler);
			Assert.IsType<ButtonHandler>(defaultHandler);
			Assert.IsType<ButtonHandlerStub>( specificHandler);
		}

		[Fact]
		public void Get10000Handlers()
		{
			int iterations = 10000;
			var app = App.CreateDefaultBuilder()
						 .Build<AppStub>();

			var handlerWarmup = app.Handlers.GetHandler<Button>();

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
	}
}

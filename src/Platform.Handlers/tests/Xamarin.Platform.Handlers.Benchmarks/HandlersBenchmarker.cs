using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xamarin.Platform.Hosting;

namespace Xamarin.Platform.Handlers.Benchmarks
{
	[MemoryDiagnoser]
	public class HandlersBenchmarker
	{
		int _numberOfItems = 100000;
		MockApp _app;
		IAppHostBuilder _builder;

		[GlobalSetup(Target = nameof(GetHandlerUsingDI))]
		public void GlobalSetupForDI()
		{
			_app = new MockApp();
			_builder = _app.CreateBuilder();
			_builder.Build(_app);
		}

		[GlobalSetup(Target = nameof(GetHandlerUsingRegistrar))]
		public void GlobalSetupForRegistrar()
		{
			Registrar.Handlers.Register<IButton, ButtonHandler>();
		}

		[IterationSetup(Target = nameof(RegisterHandlerUsingDI))]
		public void GlobalSetupForDiWithHandlersRegistration()
		{
			_builder = new AppHostBuilder();
		}

		[GlobalCleanup(Target = nameof(GetHandlerUsingDI))]
		public void GlobalCleanupForDI()
		{
			//_builder.Stop();
			//_builder.Dispose();
		}

		[Benchmark]
		public void RegisterHandlerUsingDI()
		{
			for (int i = 0; i < _numberOfItems; i++)
			{
				_builder.RegisterHandler<IButton, ButtonHandler>();
			}
		}

		[Benchmark]
		public void RegisterHandlerUsingRegistrar()
		{
			for (int i = 0; i < _numberOfItems; i++)
			{
				Registrar.Handlers.Register<IButton, ButtonHandler>();
			}
		}

		[Benchmark]
		public void GetHandlerUsingDI()
		{
			for (int i = 0; i < _numberOfItems; i++)
			{
				var defaultHandler = _app.Context.Handlers.GetHandler<IButton>();
			}
		}

		[Benchmark(Baseline = true)]
		public void GetHandlerUsingRegistrar()
		{
			for (int i = 0; i < _numberOfItems; i++)
			{
				var defaultHandler = Registrar.Handlers.GetHandler<IButton>();
			}
		}
	}

	class MockApp : App
	{
		public void ConfigureNativeServices(HostBuilderContext ctx, IServiceCollection services)
		{
			services.AddSingleton<IMauiContext>(provider => new HandlersContextStub(provider));
		}

		public override IAppHostBuilder CreateBuilder()
		{
			return base.CreateBuilder().ConfigureServices(ConfigureNativeServices);
		}
	}

	class HandlersContextStub : IMauiContext
	{
		IServiceProvider _provider;
		IMauiServiceProvider _handlersServiceProvider;
		public HandlersContextStub(IServiceProvider provider)
		{
			_provider = provider;
			_handlersServiceProvider = Provider.GetService<IMauiServiceProvider>() ?? throw new NullReferenceException(nameof(IMauiServiceProvider));
		}

		public IServiceProvider Provider => _provider;

		public IMauiServiceProvider Handlers => _handlersServiceProvider;
	}
}

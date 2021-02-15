using BenchmarkDotNet.Attributes;
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
			_builder = App.CreateDefaultBuilder();
			_app = _builder.Build<MockApp>();
		}

		[GlobalSetup(Target = nameof(GetHandlerUsingRegistrar))]
		public void GlobalSetupForRegistrar()
		{
			Registrar.Handlers.Register<IButton, ButtonHandler>();
		}

		[IterationSetup(Target = nameof(RegisterHandlerUsingDI))]
		public void GlobalSetupForDiWithHandlersRegistration()
		{
			_builder = new AppBuilder();
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
				var defaultHandler = _app.Handlers.GetHandler<IButton>();
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

	}
}

using BenchmarkDotNet.Running;

namespace Xamarin.Platform.Handlers.Benchmarks
{
	class Program
	{
		static void Main(string[] args)
		{
			var summary = BenchmarkRunner.Run<HandlersBenchmarker>();
		}
	}
}

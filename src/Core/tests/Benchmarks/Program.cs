using BenchmarkDotNet.Running;

namespace Microsoft.Maui.Handlers.Benchmarks
{
	class Program
	{
		static void Main(string[] args)
		{
			BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).RunAllJoined();
		}
	}
}

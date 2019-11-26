using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using NUnit.Framework.Api;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace Xamarin.Forms.Controls.Tests
{
	public class PlatformTestRunner
	{
		readonly ITestListener _testListener = new ControlGalleryTestListener();

		public async Task Run(ITestFilter testFilter = null)
		{
			testFilter = testFilter ?? TestFilter.Empty;

#if NETSTANDARD2_0
			var controls = Assembly.GetExecutingAssembly();
#else
			var controls = typeof(PlatformTestRunner).GetTypeInfo().Assembly;
#endif
			Assembly platform = DependencyService.Resolve<IAssemblyRetriever>().GetAssembly();

			var runner = new NUnitTestAssemblyRunner(new DefaultTestAssemblyBuilder());

			await Task.Run(() => runner.Load(controls, new Dictionary<string, object>())).ConfigureAwait(false);
			await Task.Run(() => runner.Run(_testListener, testFilter));

			await Task.Run(() => runner.Load(platform, new Dictionary<string, object>())).ConfigureAwait(false);
			await Task.Run(() => runner.Run(_testListener, testFilter));
		}
	}

	public interface IAssemblyRetriever 
	{
		Assembly GetAssembly();
	}
}

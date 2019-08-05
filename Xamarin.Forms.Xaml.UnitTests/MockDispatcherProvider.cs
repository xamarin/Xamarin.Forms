using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml.UnitTests;

[assembly: Dependency(typeof(MockDispatcherProvider))]
namespace Xamarin.Forms.Xaml.UnitTests
{
	public class MockDispatcherProvider : IDispatcherProvider
	{
		public IDispatcher GetDispatcher(object context)
		{
			return new MockDispatcher();
		}
	}

	public class MockDispatcher : IDispatcher
	{
		public void BeginInvokeOnMainThread(Action action)
		{
			Device.BeginInvokeOnMainThread(action);
		}

		bool IDispatcher.IsInvokeRequired => Device.IsInvokeRequired;
	}
}

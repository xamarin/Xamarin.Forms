using System;

using NUnit.Framework;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Core.UnitTests
{
	[TestFixture]
	public class DeviceUnitTests : BaseTestFixture
	{
		[Test]
		public void TestBeginInvokeOnMainThread()
		{
			bool calledFromMainThread = false;
			Device.PlatformServices = MockPlatformServices(() => calledFromMainThread = true);

			bool invoked = false;
			Device.BeginInvokeOnMainThread(() => invoked = true);

			Assert.True(invoked, "Action not invoked.");
			Assert.True(calledFromMainThread, "Action not invoked from main thread.");
		}

		[Test]
		public void InvokeOnMainThreadThrowsWhenNull()
		{
			Device.PlatformServices = null;
			Assert.Throws<InvalidOperationException>(() => Device.BeginInvokeOnMainThread(() => { }));
		}

		private IPlatformServices MockPlatformServices(Action onInvokeOnMainThread, Action<Action> invokeOnMainThread = null)
		{
			return new MockPlatformServices(
				invokeOnMainThread: action =>
				{
					onInvokeOnMainThread();

					if (invokeOnMainThread == null)
						action();
					else
						invokeOnMainThread(action);
				});
		}
	}
}

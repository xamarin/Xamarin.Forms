using System;
using System.Threading.Tasks;
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
		public async Task TestInvokeOnMainThreadWithSyncFunc()
		{
			bool calledFromMainThread = false;
			Device.PlatformServices = MockPlatformServices(() => calledFromMainThread = true);

			bool invoked = false;
			var result = await Device.InvokeOnMainThreadAsync(() => { invoked = true; return true; });

			Assert.True(invoked, "Action not invoked.");
			Assert.True(calledFromMainThread, "Action not invoked from main thread.");
			Assert.True(result, "Unexpected result.");
		}

		[Test]
		public async Task TestInvokeOnMainThreadWithSyncAction()
		{
			bool calledFromMainThread = false;
			Device.PlatformServices = MockPlatformServices(() => calledFromMainThread = true);

			bool invoked = false;
			await Device.InvokeOnMainThreadAsync(() => { invoked = true; });

			Assert.True(invoked, "Action not invoked.");
			Assert.True(calledFromMainThread, "Action not invoked from main thread.");
		}

		[Test]
		public async Task TestInvokeOnMainThreadWithAsyncFunc()
		{
			bool calledFromMainThread = false;
			Device.PlatformServices = MockPlatformServices(() => calledFromMainThread = true,
				invokeOnMainThread: action => Task.Delay(50).ContinueWith(_ => action()));

			bool invoked = false;
			var task = Device.InvokeOnMainThreadAsync(async () => { invoked = true; return true; });
			Assert.True(calledFromMainThread, "Action not invoked from main thread.");
			Assert.False(invoked, "Action invoked early.");

			var result = await task;
			Assert.True(invoked, "Action not invoked.");
			Assert.True(result, "Unexpected result.");
		}

		[Test]
		public async Task TestInvokeOnMainThreadWithAsyncFuncError()
		{
			bool calledFromMainThread = false;
			Device.PlatformServices = MockPlatformServices(() => calledFromMainThread = true,
				invokeOnMainThread: action => Task.Delay(50).ContinueWith(_ => action()));

			bool invoked = false;
			async Task<bool> boom() { invoked = true; throw new ApplicationException(); }
			var task = Device.InvokeOnMainThreadAsync(boom);
			Assert.True(calledFromMainThread, "Action not invoked from main thread.");
			Assert.False(invoked, "Action invoked early.");

			async Task MethodThatThrows() => await task;
			Assert.ThrowsAsync<ApplicationException>(MethodThatThrows);
			Assert.True(invoked, "Action not invoked.");
		}

		[Test]
		public async Task TestInvokeOnMainThreadWithAsyncAction()
		{
			bool calledFromMainThread = false;
			Device.PlatformServices = MockPlatformServices(() => calledFromMainThread = true,
				invokeOnMainThread: action => Task.Delay(50).ContinueWith(_ => action()));

			bool invoked = false;
			var task = Device.InvokeOnMainThreadAsync(async () => { invoked = true; });
			Assert.True(calledFromMainThread, "Action not invoked from main thread.");
			Assert.False(invoked, "Action invoked early.");

			await task;
			Assert.True(invoked, "Action not invoked.");
		}

		[Test]
		public async Task TestInvokeOnMainThreadWithAsyncActionError()
		{
			bool calledFromMainThread = false;
			Device.PlatformServices = MockPlatformServices(() => calledFromMainThread = true,
				invokeOnMainThread: action => Task.Delay(50).ContinueWith(_ => action()));

			bool invoked = false;
			async Task boom() { invoked = true; throw new ApplicationException(); }
			var task = Device.InvokeOnMainThreadAsync(boom);
			Assert.True(calledFromMainThread, "Action not invoked from main thread.");
			Assert.False(invoked, "Action invoked early.");

			async Task MethodThatThrows() => await task;
			Assert.ThrowsAsync<ApplicationException>(MethodThatThrows);
			Assert.True(invoked, "Action not invoked.");
		}

		[Test]
		public void InvokeOnMainThreadThrowsWhenNull()
		{
			Device.PlatformServices = null;
			Assert.Throws<InvalidOperationException>(() => Device.BeginInvokeOnMainThread(() => { }));
			Assert.Throws<InvalidOperationException>(() => Device.InvokeOnMainThreadAsync(() => { }).Wait(100));
			Assert.Throws<InvalidOperationException>(() => Device.InvokeOnMainThreadAsync(() => true).Wait(100));
			Assert.Throws<InvalidOperationException>(() => Device.InvokeOnMainThreadAsync(async () => { }).Wait(100));
			Assert.Throws<InvalidOperationException>(() => Device.InvokeOnMainThreadAsync(async () => true).Wait(100));
		}

		[Test]
		public async Task DeviceTimerWithSuccessfulTaskReturningTrue()
		{
			const int timerDelayInSeconds = 1;
			const int expectedNumberOfExecutions = 2;
			int numberOfSuccessfulTaskExecutions = 0;

			Device.StartTimer(TimeSpan.FromSeconds(timerDelayInSeconds), successfulFunction);

			//Wait for Timer to trigger twice
			await Task.Delay(TimeSpan.FromSeconds(timerDelayInSeconds * 2 + 1));

			Assert.AreEqual(expectedNumberOfExecutions, numberOfSuccessfulTaskExecutions);

			bool successfulFunction()
			{
				if (++numberOfSuccessfulTaskExecutions >= expectedNumberOfExecutions)
					return false;

				return true;
			}
		}

		[Test]
		public async Task DeviceTimerWithSuccessfulTaskReturningFalse()
		{
			const int timerDelayInSeconds = 1;
			const int expectedNumberOfExecutions = 1;
			int numberOfSuccessfulTaskExecutions = 0;

			Device.StartTimer(TimeSpan.FromSeconds(timerDelayInSeconds), successfulFunction);

			//Wait for Timer to trigger twice
			await Task.Delay(TimeSpan.FromSeconds(timerDelayInSeconds * 2 + 1));

			Assert.AreEqual(expectedNumberOfExecutions, numberOfSuccessfulTaskExecutions);

			bool successfulFunction()
			{
				numberOfSuccessfulTaskExecutions++;
				return false;
			}
		}

		[Test]
		public async Task AsyncDeviceTimerWithSuccessfulTaskReturningTrue()
		{
			const int timerDelayInSeconds = 1;
			const int expectedNumberOfExecutions = 2;
			int numberOfSuccessfulTaskExecutions = 0;

			Device.StartTimer(TimeSpan.FromSeconds(timerDelayInSeconds), successfulTask);

			//Wait for Timer to trigger twice
			await Task.Delay(TimeSpan.FromSeconds(timerDelayInSeconds * 2 + 1));

			Assert.AreEqual(expectedNumberOfExecutions, numberOfSuccessfulTaskExecutions);

			async Task<bool> successfulTask()
			{
				await Task.Delay(TimeSpan.FromSeconds(timerDelayInSeconds / 2));

				if (++numberOfSuccessfulTaskExecutions >= expectedNumberOfExecutions)
				{
					return false;
				}

				return true;
			}
		}

		[Test]
		public async Task AsyncDeviceTimerWithSuccessfulTaskReturningFalse()
		{
			const int timerDelayInSeconds = 1;
			const int expectedNumberOfExecutions = 1;
			int numberOfSuccessfulTaskExecutions = 0;

			Device.StartTimer(TimeSpan.FromSeconds(timerDelayInSeconds), successfulTask);

			//Wait for Timer to trigger twice
			await Task.Delay(TimeSpan.FromSeconds(timerDelayInSeconds * 2 + 1));

			Assert.AreEqual(expectedNumberOfExecutions, numberOfSuccessfulTaskExecutions);

			async Task<bool> successfulTask()
			{
				await Task.Delay(TimeSpan.FromSeconds(timerDelayInSeconds / 2));

				++numberOfSuccessfulTaskExecutions;
				return false;
			}
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

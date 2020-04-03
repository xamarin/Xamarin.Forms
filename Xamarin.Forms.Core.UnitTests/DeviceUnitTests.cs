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
		public async Task DeviceTimerWithSuccessfulFunnctionReturningTrue()
		{
			//Arrange
			const int expectedNumberOfExecutions = 2;
			var timerCompletedCompletionSource = new TaskCompletionSource<bool>();
			var timerDelay = TimeSpan.FromSeconds(1);
			var numberOfSuccessfulTaskExecutions = 0;


			//Act
			Device.StartTimer(timerDelay, successfulFunction);

			//Wait for timer to complete
			await timerCompletedCompletionSource.Task;

			//Ensure timer does not continue to run
			await Task.Delay(timerDelay.Add(timerDelay).Add(timerDelay));


			//Assert
			Assert.AreEqual(expectedNumberOfExecutions, numberOfSuccessfulTaskExecutions);

			bool successfulFunction()
			{
				if (++numberOfSuccessfulTaskExecutions >= expectedNumberOfExecutions)
				{
					timerCompletedCompletionSource.TrySetResult(true);
					return false;
				}

				return true;
			}
		}

		[Test]
		public async Task DeviceTimerWithSuccessfulFunctionReturningFalse()
		{
			//Arrange
			const int expectedNumberOfExecutions = 1;
			var timerCompletedCompletionSource = new TaskCompletionSource<bool>();
			var timerDelay = TimeSpan.FromSeconds(1);
			var numberOfSuccessfulTaskExecutions = 0;


			//Act
			Device.StartTimer(timerDelay, successfulFunction);

			//Wait for timer to complete
			await timerCompletedCompletionSource.Task;

			//Ensure timer does not continue to run
			await Task.Delay(timerDelay.Add(timerDelay).Add(timerDelay));


			//Assert
			Assert.AreEqual(expectedNumberOfExecutions, numberOfSuccessfulTaskExecutions);

			bool successfulFunction()
			{
				numberOfSuccessfulTaskExecutions++;
				timerCompletedCompletionSource.TrySetResult(true);
				return false;
			}
		}

		[Test]
		public async Task AsyncDeviceTimerWithSuccessfulShorterTaskReturningTrue()
		{
			//Arrange
			const int expectedNumberOfExecutions = 2;
			var timerCompletedCompletionSource = new TaskCompletionSource<bool>();
			var timerDelay = TimeSpan.FromSeconds(1);
			var taskFunctionDelay = TimeSpan.FromSeconds(0.5);
			int numberOfSuccessfulTaskExecutions = 0;
			var numberOfTimerTriggers = 0;


			//Act
			Device.StartTimer(timerDelay, successfulTask);

			//Wait for timer to complete
			await timerCompletedCompletionSource.Task;

			//Ensure timer does not continue to run
			await Task.Delay(timerDelay.Add(timerDelay).Add(timerDelay));


			//Assert
			Assert.AreEqual(expectedNumberOfExecutions, numberOfSuccessfulTaskExecutions);
			Assert.AreEqual(numberOfTimerTriggers, numberOfSuccessfulTaskExecutions);

			async Task<bool> successfulTask()
			{
				numberOfTimerTriggers++;

				await Task.Delay(taskFunctionDelay);

				if (++numberOfSuccessfulTaskExecutions >= expectedNumberOfExecutions)
				{
					timerCompletedCompletionSource.TrySetResult(true);
					return false;
				}

				return true;
			}
		}

		[Test]
		public async Task AsyncDeviceTimerWithSuccessfulShorterTaskReturningFalse()
		{
			//Arrange
			const int expectedNumberOfExecutions = 1;
			var timerCompletedCompletionSource = new TaskCompletionSource<bool>();
			var timerDelay = TimeSpan.FromSeconds(1);
			var taskFunctionDelay = TimeSpan.FromSeconds(0.5);
			var numberOfSuccessfulTaskExecutions = 0;
			var numberOfTimerTriggers = 0;


			//Act
			Device.StartTimer(timerDelay, successfulTask);

			//Wait for timer to complete
			await timerCompletedCompletionSource.Task;

			//Ensure timer does not continue to run
			await Task.Delay(timerDelay.Add(timerDelay).Add(timerDelay));

			//Assert
			Assert.AreEqual(expectedNumberOfExecutions, numberOfSuccessfulTaskExecutions);
			Assert.AreEqual(numberOfTimerTriggers, numberOfSuccessfulTaskExecutions);

			async Task<bool> successfulTask()
			{
				numberOfTimerTriggers++;

				await Task.Delay(taskFunctionDelay);

				++numberOfSuccessfulTaskExecutions;
				timerCompletedCompletionSource.TrySetResult(true);
				return false;
			}
		}

		[Test]
		public async Task AsyncDeviceTimerWithSuccessfulLongerTaskReturningTrue()
		{
			//Arrange
			var timerTriggeredCompletionSource = new TaskCompletionSource<bool>();
			var timerCompletedCompletionSource = new TaskCompletionSource<bool>();
			var timerDelay = TimeSpan.FromSeconds(1);
			var taskFunctionDelay = timerDelay.Add(timerDelay);
			var numberOfSuccessfulTaskExecutions = 0;
			var numberOfTimerTriggers = 0;


			//Act
			Device.StartTimer(timerDelay, successfulTask);

			//Wait for timer to trigger
			await timerTriggeredCompletionSource.Task;

			//Assert
			Assert.AreEqual(numberOfTimerTriggers, 1);
			Assert.Greater(numberOfTimerTriggers, numberOfSuccessfulTaskExecutions);

			//Act
			//Wait for timer to complete
			await timerCompletedCompletionSource.Task;

			//Ensure timer continues to run
			await Task.Delay(timerDelay.Add(taskFunctionDelay).Add(timerDelay).Add(taskFunctionDelay));

			//Assert
			Assert.Greater(numberOfTimerTriggers, 1);
			Assert.Greater(numberOfSuccessfulTaskExecutions, 1);
			Assert.GreaterOrEqual(numberOfTimerTriggers, numberOfSuccessfulTaskExecutions);

			async Task<bool> successfulTask()
			{
				numberOfTimerTriggers++;
				timerTriggeredCompletionSource.TrySetResult(true);

				await Task.Delay(taskFunctionDelay);

				numberOfSuccessfulTaskExecutions++;
				timerCompletedCompletionSource.TrySetResult(true);

				return true;
			}
		}

		[Test]
		public async Task AsyncDeviceTimerWithSuccessfulLongerTaskReturningFalse()
		{
			//Arrange
			var timerTriggeredCompletionSource = new TaskCompletionSource<bool>();
			var timerCompletedCompletionSource = new TaskCompletionSource<bool>();
			var timerDelay = TimeSpan.FromSeconds(1);
			var taskFunctionDelay = timerDelay.Add(timerDelay);
			var numberOfSuccessfulTaskExecutions = 0;
			var numberOfTimerTriggers = 0;


			//Act
			Device.StartTimer(timerDelay, successfulTask);

			//Wait for timer to trigger
			await timerTriggeredCompletionSource.Task;

			//Assert
			Assert.AreEqual(numberOfTimerTriggers, 1);
			Assert.Greater(numberOfTimerTriggers, numberOfSuccessfulTaskExecutions);

			//Act
			//Wait for timer to complete
			await timerCompletedCompletionSource.Task;

			//Ensure timer continues to run
			await Task.Delay(timerDelay.Add(taskFunctionDelay).Add(timerDelay).Add(taskFunctionDelay));


			//Assert
			Assert.Greater(numberOfSuccessfulTaskExecutions, 1);
			Assert.Greater(numberOfSuccessfulTaskExecutions, 1);
			Assert.AreEqual(numberOfTimerTriggers, numberOfSuccessfulTaskExecutions);

			async Task<bool> successfulTask()
			{
				numberOfTimerTriggers++;
				timerTriggeredCompletionSource.TrySetResult(true);

				await Task.Delay(taskFunctionDelay);

				numberOfSuccessfulTaskExecutions++;
				timerCompletedCompletionSource.TrySetResult(true);
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

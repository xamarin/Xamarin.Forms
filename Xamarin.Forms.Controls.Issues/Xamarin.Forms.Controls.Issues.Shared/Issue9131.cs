using System;
using System.Threading.Tasks;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using System.Linq;
using NUnit.Framework;
using Query = System.Func<Xamarin.UITest.Queries.AppQuery, Xamarin.UITest.Queries.AppQuery>;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 9131, "[Enhancement] Add Asynchronous Callback to Device.StartTimer", PlatformAffected.All)]
	public class Issue9131 : TestContentPage
	{
		const string ResultsLabelAutomationId = nameof(ResultsLabelAutomationId);
		const string TestRunningText = "Running 🏃‍♂️";
		const string SuccessText = "Success 😁";
		const string FailedText = "Failed 🙈";

		bool _areAllTestsCompleted = false;

#if UITEST
		[Test]
		public void RunDeviceStartTimerTests()
		{
			Query resultsLabelQuery = x => x.Marked(ResultsLabelAutomationId);

			RunningApp.WaitForElement(resultsLabelQuery);
			RunningApp.WaitForElement(TestRunningText);
			try
			{
				RunningApp.WaitForElement(SuccessText, $"{nameof(RunDeviceStartTimerTests)} Timed Out", TimeSpan.FromMinutes(2));
			}
			catch
			{
				if (RunningApp.Query(resultsLabelQuery).First().Text is TestRunningText)
					throw;

				Assert.Fail(RunningApp.Query(resultsLabelQuery).First().Text);
			}
		}
#endif
		protected override async void Init()
		{
			var resultsLabel = new Label
			{
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center,
				HorizontalTextAlignment = TextAlignment.Center,
				VerticalTextAlignment = TextAlignment.Center,
				AutomationId = ResultsLabelAutomationId,
				Text = TestRunningText
			};

			Content = resultsLabel;

			try
			{
				//Run all tests in sequence (not parallel) because Device.StartTimer relys on Main Thread
				await DeviceTimerWithSuccessfulFunnctionReturningTrue();
				await DeviceTimerWithSuccessfulFunctionReturningFalse();

				await AsyncDeviceTimerWithSuccessfulShorterTaskReturningTrue(true);
				await AsyncDeviceTimerWithSuccessfulShorterTaskReturningFalse(true);
				await AsyncDeviceTimerWithSuccessfulLongerTaskReturningTrue(true);
				await AsyncDeviceTimerWithSuccessfulLongerTaskReturningFalse(true);

				await AsyncDeviceTimerWithSuccessfulShorterTaskReturningTrue(false);
				await AsyncDeviceTimerWithSuccessfulShorterTaskReturningFalse(false);
				await AsyncDeviceTimerWithSuccessfulLongerTaskReturningTrue(false);
				await AsyncDeviceTimerWithSuccessfulLongerTaskReturningFalse(false);

				resultsLabel.Text = SuccessText;
			}
			catch (Exception e)
			{
				resultsLabel.Text = FailedText + "\n" + e.Message;
			}
			finally
			{
				_areAllTestsCompleted = true;
			}
		}

		private async Task DeviceTimerWithSuccessfulFunnctionReturningTrue()
		{
			//Arrange
			var timerCompletedCompletionSource = new TaskCompletionSource<bool>();
			const int expectedNumberOfExecutions = 2;
			var timerDelay = TimeSpan.FromSeconds(1);
			var numberOfSuccessfulTaskExecutions = 0;


			//Act
			Device.StartTimer(timerDelay, successfulFunction);

			//Wait for function to complete
			await timerCompletedCompletionSource.Task;

			//Ensure StartTimer does not continue to run 
			await Task.Delay(timerDelay.Add(timerDelay).Add(timerDelay));


			//Assert
			if (expectedNumberOfExecutions != numberOfSuccessfulTaskExecutions)
				throw new Exception($"{nameof(DeviceTimerWithSuccessfulFunnctionReturningTrue)} failed");

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

		private async Task DeviceTimerWithSuccessfulFunctionReturningFalse()
		{
			//Arrange
			var timerCompletedCompletionSource = new TaskCompletionSource<bool>();
			const int expectedNumberOfExecutions = 1;
			var timerDelay = TimeSpan.FromSeconds(1);
			var numberOfSuccessfulTaskExecutions = 0;


			//Act
			Device.StartTimer(timerDelay, successfulFunction);

			//Wait for function to complete
			await timerCompletedCompletionSource.Task;

			//Ensure StartTimer does not continue to run 
			await Task.Delay(timerDelay.Add(timerDelay).Add(timerDelay));


			//Assert
			if (expectedNumberOfExecutions != numberOfSuccessfulTaskExecutions)
				throw new Exception($"{nameof(DeviceTimerWithSuccessfulFunctionReturningFalse)} failed");

			bool successfulFunction()
			{
				numberOfSuccessfulTaskExecutions++;
				timerCompletedCompletionSource.TrySetResult(true);
				return false;
			}
		}

		private async Task AsyncDeviceTimerWithSuccessfulShorterTaskReturningTrue(bool continueOnCapturedContext)
		{
			//Arrange
			var timerCompletedCompletionSource = new TaskCompletionSource<bool>();
			const int expectedNumberOfExecutions = 2;
			var timerDelay = TimeSpan.FromSeconds(1);
			var taskFunctionDelay = TimeSpan.FromSeconds(0.5);
			int numberOfSuccessfulTaskExecutions = 0;
			var numberOfTimerTriggers = 0;


			//Act
			Device.StartTimer(timerDelay, successfulTask);

			//Wait for function to complete
			await timerCompletedCompletionSource.Task;

			//Ensure StartTimer does not continue to run 
			await Task.Delay(timerDelay.Add(timerDelay).Add(timerDelay));


			//Assert
			if (expectedNumberOfExecutions != numberOfSuccessfulTaskExecutions || numberOfTimerTriggers != numberOfSuccessfulTaskExecutions)
				throw new Exception($"{nameof(AsyncDeviceTimerWithSuccessfulShorterTaskReturningTrue)} + {nameof(continueOnCapturedContext)} {continueOnCapturedContext} failed");

			async Task<bool> successfulTask()
			{
				numberOfTimerTriggers++;

				await Task.Delay(taskFunctionDelay).ConfigureAwait(continueOnCapturedContext);

				if (++numberOfSuccessfulTaskExecutions >= expectedNumberOfExecutions)
				{
					timerCompletedCompletionSource.TrySetResult(true);
					return false;
				}

				return true;
			}
		}

		private async Task AsyncDeviceTimerWithSuccessfulShorterTaskReturningFalse(bool continueOnCapturedContext)
		{
			//Arrange
			var timerCompletedCompletionSource = new TaskCompletionSource<bool>();
			const int expectedNumberOfExecutions = 1;
			var timerDelay = TimeSpan.FromSeconds(1);
			var taskFunctionDelay = TimeSpan.FromSeconds(0.5);
			var numberOfSuccessfulTaskExecutions = 0;
			var numberOfTimerTriggers = 0;


			//Act
			Device.StartTimer(timerDelay, successfulTask);

			//Wait for function to complete
			await timerCompletedCompletionSource.Task;

			//Ensure StartTimer does not continue to run 
			await Task.Delay(timerDelay.Add(timerDelay).Add(timerDelay));


			//Assert
			if (expectedNumberOfExecutions != numberOfSuccessfulTaskExecutions || numberOfTimerTriggers != numberOfSuccessfulTaskExecutions)
				throw new Exception($"{nameof(AsyncDeviceTimerWithSuccessfulShorterTaskReturningFalse)} + {nameof(continueOnCapturedContext)} {continueOnCapturedContext} failed");

			async Task<bool> successfulTask()
			{
				numberOfTimerTriggers++;

				await Task.Delay(taskFunctionDelay).ConfigureAwait(continueOnCapturedContext);

				++numberOfSuccessfulTaskExecutions;
				timerCompletedCompletionSource.TrySetResult(true);
				return false;
			}
		}

		private async Task AsyncDeviceTimerWithSuccessfulLongerTaskReturningTrue(bool continueOnCapturedContext)
		{
			//Arrange
			var timerTriggeredCompletionSource = new TaskCompletionSource<bool>();
			var functionCompletedCompletionSource = new TaskCompletionSource<bool>();
			var timerDelay = TimeSpan.FromSeconds(1);
			var taskFunctionDelay = timerDelay.Add(timerDelay);
			var numberOfSuccessfulTaskExecutions = 0;
			var numberOfTimerTriggers = 0;


			//Act
			Device.StartTimer(timerDelay, successfulTask);

			//Wait for timer to trigger
			await timerTriggeredCompletionSource.Task;


			//Assert
			if (numberOfTimerTriggers <= numberOfSuccessfulTaskExecutions || numberOfSuccessfulTaskExecutions > 0)
				throw new Exception($"{nameof(AsyncDeviceTimerWithSuccessfulLongerTaskReturningTrue)} + {nameof(continueOnCapturedContext)} {continueOnCapturedContext} failed");

			//Act
			//Ensure StartTimer continues to run
			await functionCompletedCompletionSource.Task;
			await Task.Delay(taskFunctionDelay.Add(taskFunctionDelay).Add(taskFunctionDelay));


			//Assert
			if (numberOfTimerTriggers < numberOfSuccessfulTaskExecutions || numberOfSuccessfulTaskExecutions <= 0)
				throw new Exception($"{nameof(AsyncDeviceTimerWithSuccessfulLongerTaskReturningTrue)} + {nameof(continueOnCapturedContext)} {continueOnCapturedContext} failed");

			async Task<bool> successfulTask()
			{
				numberOfTimerTriggers++;

				timerTriggeredCompletionSource.TrySetResult(true);
				await Task.Delay(taskFunctionDelay).ConfigureAwait(continueOnCapturedContext);

				numberOfSuccessfulTaskExecutions++;
				functionCompletedCompletionSource.TrySetResult(true);

				//Return false once all tests are completed to end Device.StartTimer and avoid it interferring with other tests
				return true && !_areAllTestsCompleted;
			}
		}

		private async Task AsyncDeviceTimerWithSuccessfulLongerTaskReturningFalse(bool continueOnCapturedContext)
		{
			//Arrange
			var timerTriggeredCompletionSource = new TaskCompletionSource<bool>();
			var functionCompletedCompletionSource = new TaskCompletionSource<bool>();
			var timerDelay = TimeSpan.FromSeconds(1);
			var taskFunctionDelay = timerDelay.Add(timerDelay);
			var numberOfSuccessfulTaskExecutions = 0;
			var numberOfTimerTriggers = 0;


			//Act
			Device.StartTimer(timerDelay, successfulTask);

			//Wait for function to complete
			await timerTriggeredCompletionSource.Task;

			//Assert
			if (numberOfTimerTriggers <= numberOfSuccessfulTaskExecutions || numberOfSuccessfulTaskExecutions > 0)
				throw new Exception($"{nameof(AsyncDeviceTimerWithSuccessfulLongerTaskReturningFalse)} + {nameof(continueOnCapturedContext)} {continueOnCapturedContext} failed");

			//Ensure StartTimer does not continue to run
			await functionCompletedCompletionSource.Task;
			await Task.Delay(taskFunctionDelay.Add(taskFunctionDelay).Add(taskFunctionDelay));


			//Assert
			if (numberOfTimerTriggers < numberOfSuccessfulTaskExecutions || numberOfSuccessfulTaskExecutions <= 0)
				throw new Exception($"{nameof(AsyncDeviceTimerWithSuccessfulLongerTaskReturningFalse)} + {nameof(continueOnCapturedContext)} {continueOnCapturedContext} failed");

			async Task<bool> successfulTask()
			{
				numberOfTimerTriggers++;
				timerTriggeredCompletionSource.TrySetResult(true);

				await Task.Delay(taskFunctionDelay).ConfigureAwait(continueOnCapturedContext);

				numberOfSuccessfulTaskExecutions++;
				functionCompletedCompletionSource.TrySetResult(true);
				return false;
			}
		}
	}
}

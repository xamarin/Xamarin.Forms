using System;
using System.Threading.Tasks;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
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
		const string TestRunningText = "Running";
		const string SuccessText = "Success";
		const string FailedText = "Failed";

#if UITEST
		[Test]
		public void RunDeviceStartTimerTests()
		{
			Query resultsLabelQuery = x => x.Marked(ResultsLabelAutomationId);

			RunningApp.WaitForElement(resultsLabelQuery);
			RunningApp.WaitForElement(TestRunningText);
			RunningApp.WaitForElement(SuccessText, timeout: TimeSpan.FromMinutes(2));
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
				await AsyncDeviceTimerWithSuccessfulShorterTaskReturningTrue();
				await AsyncDeviceTimerWithSuccessfulShorterTaskReturningFalse();
				await AsyncDeviceTimerWithSuccessfulLongerTaskReturningTrue();
				await AsyncDeviceTimerWithSuccessfulLongerTaskReturningFalse();

				resultsLabel.Text = SuccessText;
			}
			catch
			{
				resultsLabel.Text = FailedText;
			}
		}

		private async Task DeviceTimerWithSuccessfulFunnctionReturningTrue()
		{
			const int expectedNumberOfExecutions = 2;
			var timerDelay = TimeSpan.FromSeconds(1);
			var numberOfSuccessfulTaskExecutions = 0;

			Device.StartTimer(timerDelay, successfulFunction);

			//Wait for Timer to trigger at least twice
			await Task.Delay(timerDelay.Add(timerDelay).Add(timerDelay));

			if (expectedNumberOfExecutions != numberOfSuccessfulTaskExecutions)
				throw new Exception($"{nameof(DeviceTimerWithSuccessfulFunnctionReturningTrue)} failed");

			bool successfulFunction()
			{
				if (++numberOfSuccessfulTaskExecutions >= expectedNumberOfExecutions)
					return false;

				return true;
			}
		}

		private async Task DeviceTimerWithSuccessfulFunctionReturningFalse()
		{
			const int expectedNumberOfExecutions = 1;
			var timerDelay = TimeSpan.FromSeconds(1);
			var numberOfSuccessfulTaskExecutions = 0;

			Device.StartTimer(timerDelay, successfulFunction);

			//Wait for Timer to trigger at least twice
			await Task.Delay(timerDelay.Add(timerDelay).Add(timerDelay));

			if (expectedNumberOfExecutions != numberOfSuccessfulTaskExecutions)
				throw new Exception($"{nameof(DeviceTimerWithSuccessfulFunctionReturningFalse)} failed");

			bool successfulFunction()
			{
				numberOfSuccessfulTaskExecutions++;
				return false;
			}
		}

		private async Task AsyncDeviceTimerWithSuccessfulShorterTaskReturningTrue()
		{
			const int expectedNumberOfExecutions = 2;
			var timerDelay = TimeSpan.FromSeconds(1);
			var taskFunctionDelay = TimeSpan.FromSeconds(0.5);
			int numberOfSuccessfulTaskExecutions = 0;
			var numberOfTimerTriggers = 0;

			Device.StartTimer(timerDelay, successfulTask);

			//Wait for Timer to trigger at least twice
			await Task.Delay(timerDelay.Add(timerDelay).Add(timerDelay));

			if (expectedNumberOfExecutions != numberOfSuccessfulTaskExecutions)
				throw new Exception($"{nameof(AsyncDeviceTimerWithSuccessfulShorterTaskReturningTrue)} failed");
			if (numberOfTimerTriggers != numberOfSuccessfulTaskExecutions)
				throw new Exception($"{nameof(AsyncDeviceTimerWithSuccessfulShorterTaskReturningTrue)} failed");

			async Task<bool> successfulTask()
			{
				numberOfTimerTriggers++;

				await Task.Delay(taskFunctionDelay);

				if (++numberOfSuccessfulTaskExecutions >= expectedNumberOfExecutions)
					return false;

				return true;
			}
		}

		private async Task AsyncDeviceTimerWithSuccessfulShorterTaskReturningFalse()
		{
			const int expectedNumberOfExecutions = 1;
			var timerDelay = TimeSpan.FromSeconds(1);
			var taskFunctionDelay = TimeSpan.FromSeconds(0.5);
			var numberOfSuccessfulTaskExecutions = 0;
			var numberOfTimerTriggers = 0;

			Device.StartTimer(timerDelay, successfulTask);

			//Wait for Timer to trigger at least twice
			await Task.Delay(timerDelay.Add(timerDelay).Add(timerDelay));

			if (expectedNumberOfExecutions != numberOfSuccessfulTaskExecutions)
				throw new Exception($"{nameof(AsyncDeviceTimerWithSuccessfulShorterTaskReturningFalse)} failed");

			if (numberOfTimerTriggers != numberOfSuccessfulTaskExecutions)
				throw new Exception($"{nameof(AsyncDeviceTimerWithSuccessfulShorterTaskReturningFalse)} failed");

			async Task<bool> successfulTask()
			{
				numberOfTimerTriggers++;

				await Task.Delay(taskFunctionDelay);

				++numberOfSuccessfulTaskExecutions;
				return false;
			}
		}

		private async Task AsyncDeviceTimerWithSuccessfulLongerTaskReturningTrue()
		{
			var timerDelay = TimeSpan.FromSeconds(1);
			var taskFunctionDelay = TimeSpan.FromSeconds(1.5);
			var numberOfSuccessfulTaskExecutions = 0;
			var numberOfTimerTriggers = 0;

			Device.StartTimer(timerDelay, successfulTask);

			//Wait for Timer to trigger at least twice
			await Task.Delay(timerDelay.Add(taskFunctionDelay));

			if (numberOfTimerTriggers <= numberOfSuccessfulTaskExecutions)
				throw new Exception($"{nameof(AsyncDeviceTimerWithSuccessfulLongerTaskReturningTrue)} failed");

			async Task<bool> successfulTask()
			{
				numberOfTimerTriggers++;

				await Task.Delay(taskFunctionDelay);

				numberOfSuccessfulTaskExecutions++;

				return true;
			}
		}

		private async Task AsyncDeviceTimerWithSuccessfulLongerTaskReturningFalse()
		{
			var timerDelay = TimeSpan.FromSeconds(1);
			var taskFunctionDelay = TimeSpan.FromSeconds(1.5);
			var numberOfSuccessfulTaskExecutions = 0;
			var numberOfTimerTriggers = 0;

			Device.StartTimer(timerDelay, successfulTask);

			//Wait for Timer to trigger at least twice
			await Task.Delay(timerDelay.Add(taskFunctionDelay));

			if (numberOfTimerTriggers <= numberOfSuccessfulTaskExecutions)
				throw new Exception($"{nameof(AsyncDeviceTimerWithSuccessfulLongerTaskReturningFalse)} failed");

			async Task<bool> successfulTask()
			{
				numberOfTimerTriggers++;

				await Task.Delay(taskFunctionDelay);

				numberOfSuccessfulTaskExecutions++;
				return false;
			}
		}
	}
}

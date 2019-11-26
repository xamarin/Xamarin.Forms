using System.Diagnostics;
using NUnit.Framework.Interfaces;

namespace Xamarin.Forms.Controls.Tests
{
	public class ControlGalleryTestListener : ITestListener
	{
		public void SendMessage(TestMessage message)
		{
			Debug.WriteLine(message);
		}

		public void TestFinished(ITestResult result)
		{
			Debug.WriteLine($"{result.Name} finished");
			MessagingCenter.Send(result, "TestFinished");
		}

		public void TestOutput(TestOutput output)
		{
			Debug.WriteLine(output);
		}

		public void TestStarted(ITest test)
		{
			Debug.WriteLine($"{test.Name} started");
			MessagingCenter.Send(test, "TestStarted");
		}
	}
}
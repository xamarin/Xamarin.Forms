using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace Xamarin.Forms.Controls.Tests
{
	public class ControlGalleryTestListener : ITestListener
	{
		public void SendMessage(TestMessage message)
		{
		}

		public void TestFinished(ITestResult result)
		{
			var test = result.Test;
			if (test is TestAssembly testAssembly)
			{
				MessagingCenter.Send(result, "AssemblyFinished");
				ExportResults(result, testAssembly.FullName);
			}
			else
			{
				MessagingCenter.Send(result, "TestFinished");
			}
		}

		void ExportResults(ITestResult result, string assemblyName) 
		{
			var stringBuilder = new StringBuilder();
			var xmlWriter = new XmlTextWriter(new StringWriter(stringBuilder));
			result.ToXml(true).WriteTo(xmlWriter);
			var xmlResult = stringBuilder.ToString();

			if (assemblyName.StartsWith("/Xamarin.Forms.Controls"))
			{
				System.Diagnostics.Debug.WriteLine($">>>>>> Setting cross platform results");
				(Application.Current as App).CrossPlatformTestResults = xmlResult;
			}
			else
			{
				System.Diagnostics.Debug.WriteLine($">>>>>> Setting native platform results");
				(Application.Current as App).NativePlatformTestResults = xmlResult;
			}
		}

		public void TestOutput(TestOutput output)
		{
			Debug.WriteLine(output);
		}

		public void TestStarted(ITest test)
		{
			MessagingCenter.Send(test, "TestStarted");
		}
	}
}
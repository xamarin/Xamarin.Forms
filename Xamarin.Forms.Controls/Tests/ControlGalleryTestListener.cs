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

				var sb = new StringBuilder();
				var tw = new StringWriter(sb);
				var xw = new XmlTextWriter(tw);
				result.ToXml(true).WriteTo(xw);

				var s = sb.ToString();

				System.Diagnostics.Debug.WriteLine(s);
			}
			else
			{
				MessagingCenter.Send(result, "TestFinished");
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
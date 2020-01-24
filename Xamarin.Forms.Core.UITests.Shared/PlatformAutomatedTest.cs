using System;
using NUnit.Framework;

namespace Xamarin.Forms.Core.UITests.Shared
{
	internal class PlatformAutomatedTest : BaseTestFixture
	{
		protected override void NavigateToGallery()
		{
			App.NavigateToGallery(GalleryQueries.PlatformAutomatedTestsGallery);
		}

		[Test]
		[Category(UITestCategories.ViewBaseTests)]
		public void AutomatedTests()
		{
			App.WaitForElement("SUCCESS", timeout: TimeSpan.FromMinutes(1));
		}

		protected override void TestTearDown()
		{
			base.TestTearDown();
			//
			/* See if we can set CurrentResult here to a custom subclass of TestResult/TestSuiteResult
			 * which we can construct with an XML string and that overrides AddToXml with that XML string
			 * If that works and the results are legible, we can retrieve the actual device results XML 
			 * via the mechanism in ContolGalleryTestListener + IApp.Invoke
			 */
			//NUnit.Framework.Internal.TestExecutionContext.CurrentContext.CurrentResult
		}
	}
}

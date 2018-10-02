using System;
using NUnit.Framework;

namespace Xamarin.Forms.Core.UITests
{
	[TestFixture]
	[Category(UITestCategories.AutomationId)]
	internal class AutomationMDPTest : BaseTestFixture
	{
		public AutomationMDPTest()
		{
		}

		protected override void NavigateToGallery()
		{

		}


		[Test]
		public void TestMDPButton()
		{
			App.Tap(c => c.Marked("btnMDPAutomationID"));
		}
	}
}

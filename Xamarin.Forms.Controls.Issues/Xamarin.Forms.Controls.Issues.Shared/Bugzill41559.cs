using System;

using Xamarin.Forms;
using Xamarin.Forms.CustomAttributes;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Issue(IssueTracker.Bugzilla, 41559, "In iOS Entry.keyboard=Keyboard.Numeric does not have a done button")]
	public class Bugzill41559 : TestContentPage
	{
		protected override void Init()
		{
			var numericEntry = new Entry { Keyboard = Keyboard.Numeric, AutomationId = "NumericEntry" };
			Content = new StackLayout
			{
				Children = {
					numericEntry
				}
			};

			numericEntry.Focus();
		}

#if UITEST
		[Test]
		public void Bugzilla41559Test ()
		{
			RunningApp.Tap("NumericEntry");
			var buttonCount = RunningApp.Query("Done").Count();
			Assert.GreaterOrEqual(buttonCount, 1, "Done keyboard button is not added");
		}
#endif
	}
}



﻿using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
#if UITEST
	[NUnit.Framework.Category(Core.UITests.UITestCategories.Bugzilla)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Bugzilla, 46494, "Hardware/Software back button from MainPage of type MasterDetail causes crash 'java.lang.IllegalStateException: Activity has been destroyed'", PlatformAffected.Android)]
	public class Bugzilla46494 : TestFlyoutPage
	{
		protected override void Init()
		{
			Flyout = new ContentPage { Title = "Flyout", BackgroundColor = Color.Blue };
			Detail = new NavigationPage(
				new ContentPage
				{
					Title = "Detail",
					BackgroundColor = Color.Red,
					Content = new ContentView
					{
						Content = new Label
						{
							Text = "Hit Back button to destroy Activity. Disposing Fragment should not run into a race condition with Activity destroy.",
							HorizontalTextAlignment = TextAlignment.Center,
							VerticalTextAlignment = TextAlignment.Center
						}
					}
				}
			);
		}
	}
}

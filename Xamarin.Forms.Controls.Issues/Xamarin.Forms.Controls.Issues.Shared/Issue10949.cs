using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using System;
using System.Collections.ObjectModel;

#if UITEST
using Xamarin.Forms.Core.UITests;
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{

#if UITEST
	[Category(UITestCategories.ActivityIndicator)]
#endif

	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 10949, "UWP replace Progressbar with ProgressRing", PlatformAffected.UWP)]
	public class Issue10949 : ContentPage
	{
		public Issue10949()
		{
			var loadingLabel = new Label
			{
				Text = "Loading"
			};

			var activityIndicator = new ActivityIndicator()
			{
				IsRunning = true,
				IsVisible = true,
				WidthRequest = 100,
				HeightRequest = 100,
				Color = Color.Red,
				BackgroundColor = Color.Yellow
			};
			// Build the page.
			Content = new StackLayout
			{
				Orientation = StackOrientation.Vertical,
				Children =
				{
				     loadingLabel,
					activityIndicator
				}
			};
		}
	}
}

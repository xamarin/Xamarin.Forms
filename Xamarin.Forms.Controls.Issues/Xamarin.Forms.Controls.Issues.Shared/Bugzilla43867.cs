﻿using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Bugzilla, 43867, "Numeric keyboard shows text / default keyboard when back button is hit", PlatformAffected.Android)]
	public class Bugzilla43867 : TestContentPage // or TestMasterDetailPage, etc ...
	{
		protected override void Init()
		{
			Application.Current.On<Android>().UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Resize);

			Content = new StackLayout
			{
				Spacing = 10,
				VerticalOptions = LayoutOptions.Center,
				Children =
				{
					new Label
					{
						Text = "Focus and unfocus each element 10 times. Observe that the soft keyboard does not change layout while hiding."
					},
					new Entry
					{
						WidthRequest = 250,
						HeightRequest = 50,
						BackgroundColor = Color.AntiqueWhite
					},
					new Editor
					{
						WidthRequest = 250,
						HeightRequest = 50,
						BackgroundColor = Color.BurlyWood
					}
				}
			};
		}
	}
}
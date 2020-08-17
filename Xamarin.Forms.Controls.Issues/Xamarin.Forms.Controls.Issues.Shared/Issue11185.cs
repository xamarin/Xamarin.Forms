﻿using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.Forms.Core.UITests;
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
#if UITEST
	[Category(UITestCategories.ManualReview)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 11185, "ScrollViewRenderer HorizontalFadingEdgeEnabled ignored on horizontal ScrollView orientation", PlatformAffected.Android)]
	public class Issue11185 : TestContentPage // or TestMasterDetailPage, etc ...
	{
		protected override void Init()
		{
			var layout = new StackLayout
			{
				Orientation = StackOrientation.Horizontal
			};

			for (int i = 0; i < 20; i++)
			{
				layout.Children.Add(new BoxView { WidthRequest = 100, HeightRequest = 100, BackgroundColor = Color.LightCoral });
			}

			Content = new ScrolView11185
			{
				Orientation = ScrollOrientation.Horizontal,
				Content = layout
			};
		}
	}

	public class ScrolView11185 : ScrollView
	{

	}
}
﻿using System.Collections.Generic;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
#if UITEST
	[Category(UITestCategories.CarouselView)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 11018,
		"[Bug] IndicatorView HideSingle does not work on Android",
		PlatformAffected.Android)]
	public class Issue11018 : TestContentPage
	{
		public Issue11018()
		{
#if APP
			Title = "Issue 11018";
#endif
		}

		protected override void Init()
		{
			var layout = new StackLayout
			{
				Padding = 12
			};

			var instructions = new Label
			{
				Padding = 12,
				BackgroundColor = Color.Black,
				TextColor = Color.White,
				Text = "If you can't see the IndicatorView, the test has passed."
			};

			var settingsLayout = new StackLayout
			{
				Orientation = StackOrientation.Horizontal
			};

			var checkBox = new CheckBox
			{
				IsChecked = true,
				VerticalOptions = LayoutOptions.Center
			};

			var infoLabel = new Label
			{
				Text = "HideSingle",
				VerticalOptions = LayoutOptions.Center
			};

			settingsLayout.Children.Add(checkBox);
			settingsLayout.Children.Add(infoLabel);

			var indicatorView = new IndicatorView
			{
				HorizontalOptions = LayoutOptions.Center,
				IndicatorColor = Color.Black,
				SelectedIndicatorColor = Color.DarkGray,
				IndicatorSize = 10,
				Count = 1,
				HideSingle = true
			};

			layout.Children.Add(instructions);
			layout.Children.Add(settingsLayout);
			layout.Children.Add(indicatorView);

			Content = layout;

			checkBox.CheckedChanged += (sender, args) =>
			{
				indicatorView.HideSingle = checkBox.IsChecked;
			};
		}
	}
}

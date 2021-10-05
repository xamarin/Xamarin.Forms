﻿using System.Collections.Generic;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Shapes;

#if UITEST
using Xamarin.Forms.Core.UITests;
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
#if UITEST
	[Category(UITestCategories.Shape)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 11924, "[Bug] Shapes not loading in Xamarin ShellContent Tab once it is navigated back from other Tab",
		PlatformAffected.All)]
	public class Issue11924 : TestShell
	{
		const string Test1 = "Test 1";
		const string Test2 = "Test 2";

		public Issue11924()
		{

		}

		protected override void Init()
		{
			AddBottomTab(CreatePage1(Test1), Test1);
			AddBottomTab(CreatePage2(Test2), Test2);

			static ContentPage CreatePage1(string title)
			{
				var layout = new StackLayout();

				var instructions = new Label
				{
					Padding = 12,
					BackgroundColor = Color.Black,
					TextColor = Color.White,
					Text = "Navigate to the second Tab"
				};

				var ellipse = new Ellipse
				{
					HorizontalOptions = LayoutOptions.Start,
					HeightRequest = 50,
					WidthRequest = 100,
					Fill = Brush.Red
				};

				layout.Children.Add(instructions);
				layout.Children.Add(ellipse);

				return new ContentPage
				{
					Title = title,
					Content = layout
				};
			}

			static ContentPage CreatePage2(string title)
			{
				var layout = new StackLayout();

				var instructions = new Label
				{
					Padding = 12,
					BackgroundColor = Color.Black,
					TextColor = Color.White,
					Text = "Navigate back to the first tab, and verify if the Ellipse is rendering or not."
				};

				layout.Children.Add(instructions);

				return new ContentPage
				{
					Title = title,
					Content = layout
				};
			}
		}
	}
}
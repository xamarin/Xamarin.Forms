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
	[Category(UITestCategories.SwipeView)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 8973, "SwipeView no animation when revealing CollectionView items", PlatformAffected.Android)]
	public class Issue8973 : TestContentPage
	{
		protected override void Init()
		{
			Title = "Issue 8973";

			var swipeLayout = new StackLayout
			{
				Margin = new Thickness(12)
			};

			var instructions = new Label
			{
				BackgroundColor = Color.Black,
				TextColor = Color.White,
				Text = "Swipe to the right and verify that the opening animation of the SwipeView is correct."
			};

			var swipeItem = new SwipeItem
			{
				BackgroundColor = Color.Red,
				IconImageSource = "calculator.png",
				Text = "File"
			};

			swipeItem.Invoked += (sender, e) => { DisplayAlert("SwipeView", "File Invoked", "Ok"); };

			var swipeItems = new SwipeItems { swipeItem };

			swipeItems.Mode = SwipeMode.Reveal;

			var swipeContent = new Grid();

			var swipeLabel = new Label
			{
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center,
				Text = "Swipe to Right (File)"
			};

			swipeContent.Children.Add(swipeLabel);

			var swipeView = new SwipeView
			{
				HeightRequest = 60,
				WidthRequest = 300,
				LeftItems = swipeItems,
				Content = swipeContent
			};

			swipeLayout.Children.Add(instructions);
			swipeLayout.Children.Add(swipeView);

			Content = swipeLayout;
		}
	}
}
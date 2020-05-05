using System.Collections.Generic;
using Xamarin.Forms.CustomAttributes;
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
	[Issue(IssueTracker.Github, 10563, "[Bug] SwipeView Open methods does not work for RightItems", PlatformAffected.Android | PlatformAffected.iOS)]
	public class Issue10563 : TestContentPage
	{
		public Issue10563()
		{
#if APP
			Device.SetFlags(new List<string>(Device.Flags ?? new List<string>()) { "SwipeView_Experimental" });
#endif			
		}

		protected override void Init()
		{
			Title = "Issue 10563";

			var swipeLayout = new StackLayout
			{
				Margin = new Thickness(12)
			};

			var openLeftButton = new Button
			{
				Text = "Open Left SwipeItem"
			};

			var openRightButton = new Button
			{
				Text = "Open Right SwipeItem"
			};

			var openTopButton = new Button
			{
				Text = "Open Top SwipeItem"
			};

			var openBottomButton = new Button
			{
				Text = "Open Bottom SwipeItem"
			};

			var closeButton = new Button
			{
				Text = "Close SwipeView"
			};

			swipeLayout.Children.Add(openLeftButton);
			swipeLayout.Children.Add(openRightButton);
			swipeLayout.Children.Add(openTopButton);
			swipeLayout.Children.Add(openBottomButton);
			swipeLayout.Children.Add(closeButton);

			var swipeItem = new SwipeItem
			{
				BackgroundColor = Color.Red,
				IconImageSource = "calculator.png",
				Text = "Issue 10563"
			};

			swipeItem.Invoked += (sender, e) => { DisplayAlert("SwipeView", "SwipeItem Invoked", "Ok"); };

			var swipeItems = new SwipeItems { swipeItem };

			swipeItems.Mode = SwipeMode.Reveal;

			var swipeContent = new Grid
			{
				BackgroundColor = Color.Gray
			};

			var swipeLabel = new Label
			{
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center,
				Text = "Swipe to any direction"
			};

			swipeContent.Children.Add(swipeLabel);

			var swipeView = new SwipeView
			{
				HeightRequest = 60,
				WidthRequest = 300,
				LeftItems = swipeItems,
				RightItems = swipeItems,
				TopItems = swipeItems,
				BottomItems = swipeItems,
				Content = swipeContent
			};

			swipeLayout.Children.Add(swipeView);

			Content = swipeLayout;

			openLeftButton.Clicked += (sender, e) =>
			{
				swipeView.Open(OpenSwipeItem.LeftItems);
			};

			openRightButton.Clicked += (sender, e) =>
			{
				swipeView.Open(OpenSwipeItem.RightItems);
			};

			openTopButton.Clicked += (sender, e) =>
			{
				swipeView.Open(OpenSwipeItem.TopItems);
			};

			openBottomButton.Clicked += (sender, e) =>
			{
				swipeView.Open(OpenSwipeItem.BottomItems);
			};

			closeButton.Clicked += (sender, e) =>
			{
				swipeView.Close();
			};
		}
	}
}
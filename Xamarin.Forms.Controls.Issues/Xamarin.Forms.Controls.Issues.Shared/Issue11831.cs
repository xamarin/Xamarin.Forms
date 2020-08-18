using Xamarin.Forms.Internals;
using Xamarin.Forms.CustomAttributes;
using System.Collections.Generic;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
#if UITEST
	[Category(UITestCategories.SwipeView)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 11831,
		"[Bug] SwipeView removes Frame borders on Android",
		PlatformAffected.Android)]
	public class Issue11831 : TestContentPage
	{
		public Issue11831()
		{
#if APP
			Device.SetFlags(new List<string> { ExperimentalFlags.SwipeViewExperimental });
#endif
		}

		protected override void Init()
		{
			Title = "Issue 11831";

			var layout = new StackLayout();

			var instructions = new Label
			{
				Padding = 12,
				BackgroundColor = Color.Black,
				TextColor = Color.White,
				Text = "If the Frame SwipeView's Content has a border, the test has passed."
			};

			var contentLayout = new StackLayout
			{
				Margin = new Thickness(12)
			};

			var frameContent = new Label
			{
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center,
				Text = "Frame Content"
			};

			var frame = new Frame
			{
				HasShadow = false,
				BorderColor = Color.Red,
				BackgroundColor = Color.LightBlue,
				CornerRadius = 12
			};

			frame.Content = frameContent;

			var swipeViewContent = new Label
			{
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center,
				Text = "SwipeView Content"
			};

			var swipeViewFrame = new Frame
			{
				HasShadow = false,
				BorderColor = Color.Red,
				BackgroundColor = Color.LightBlue,
				CornerRadius = 12
			};

			swipeViewFrame.Content = swipeViewContent;

			var swipeView = new SwipeView
			{
				BackgroundColor = Color.Transparent,
				Content = swipeViewFrame
			};

			var swipeItem = new SwipeItem
			{
				BackgroundColor = Color.Red,
				Text = "SwipeItem"
			};

			swipeView.LeftItems = new SwipeItems
			{
				swipeItem
			};

			contentLayout.Children.Add(frame);
			contentLayout.Children.Add(swipeView);

			layout.Children.Add(instructions);
			layout.Children.Add(contentLayout);

			Content = layout;
		}
	}
}
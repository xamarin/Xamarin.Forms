using Xamarin.Forms.Internals;
using Xamarin.Forms.CustomAttributes;
using System.Collections.Generic;

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
	[Issue(IssueTracker.Github, 10366, "[Enhancement] Change SwipeView to Support RTL", PlatformAffected.Android)]
	public class Issue10366 : TestContentPage
	{
		const string SwipeViewId = "SwipeViewId";

		public Issue10366()
		{
			Title = "Issue 10366";

			var layout = new StackLayout
			{
				Padding = 12
			};

			var addSwipeItem = new SwipeItem { BackgroundColor = Color.Green, Text = "Add", IconImageSource = "calculator.png" };
			var editSwipeItem = new SwipeItem { BackgroundColor = Color.Orange, Text = "Favourite", IconImageSource = "bell.png" };
			var deleteSwipeItem = new SwipeItem { BackgroundColor = Color.Red, Text = "Delete", IconImageSource = "coffee.png" };

			addSwipeItem.Invoked += (sender, e) =>
			{
				DisplayAlert("SwipeView", "Add Invoked", "OK");
			};

			editSwipeItem.Invoked += (sender, e) =>
			{
				DisplayAlert("SwipeView", "Favourite Invoked", "OK");
			};

			deleteSwipeItem.Invoked += (sender, e) =>
			{
				DisplayAlert("SwipeView", "Delete Invoked", "OK");
			};

			var swipeView = new SwipeView
			{
				AutomationId = SwipeViewId,
				HeightRequest = 60,
				BackgroundColor = Color.LightGray,
				LeftItems = new SwipeItems(new List<SwipeItem> { addSwipeItem, editSwipeItem })
				{
					Mode = SwipeMode.Reveal
				},
				RightItems = new SwipeItems(new List<SwipeItem> { deleteSwipeItem })
				{
					Mode = SwipeMode.Reveal
				}
			};

			var content = new Grid
			{
				BackgroundColor = Color.LightGoldenrodYellow
			};

			var info = new Label
			{
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center,
				Text = "Swipe to the Left or Right"
			};

			content.Children.Add(info);

			swipeView.Content = content;

			var flowDirectionLayout = new StackLayout
			{
				Orientation = StackOrientation.Horizontal
			};

			var flowDirectionButton = new Button
			{
				Text = "Change FlowDirection",
				VerticalOptions = LayoutOptions.Center
			};

			var flowDirectionInfo = new Label
			{
				Text = swipeView.FlowDirection.ToString(),
				VerticalOptions = LayoutOptions.Center
			};

			flowDirectionLayout.Children.Add(flowDirectionButton);
			flowDirectionLayout.Children.Add(flowDirectionInfo);

			layout.Children.Add(swipeView);
			layout.Children.Add(flowDirectionLayout);

			Content = layout;

			flowDirectionButton.Clicked += (sender, args) =>
			{
				if (swipeView.FlowDirection == FlowDirection.RightToLeft)
					swipeView.FlowDirection = FlowDirection.LeftToRight;
				else
					swipeView.FlowDirection = FlowDirection.RightToLeft;

				flowDirectionInfo.Text = swipeView.FlowDirection.ToString();
			};
		}

		protected override void Init()
		{

		}

#if UITEST
		[Test]
		public void Issue10366TestSwipeViewRTL()
		{
			RunningApp.WaitForElement(x => x.Marked(SwipeViewId));
			RunningApp.SwipeRightToLeft(SwipeViewId);
			RunningApp.Screenshot("LTR SwipeView");
			RunningApp.Tap(x => x.Marked("Change FlowDirection"));
			RunningApp.SwipeRightToLeft(SwipeViewId);
			RunningApp.WaitForElement (q => q.Marked ("RightToLeft"));
			RunningApp.Screenshot("RTL SwipeView");
		}
#endif
	}
}
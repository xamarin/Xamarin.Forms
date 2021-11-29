using System;
using System.Collections.Generic;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 14528, "[Bug] [iOS] SwipeView shifted in direction of swipe permanently", PlatformAffected.iOS)]
	public class Issue14528 : TestContentPage
	{
		public Issue14528()
		{
			Title = "Issue 14528";
		}

		protected override void Init()
		{
			var listView = new Xamarin.Forms.ListView
			{
				HasUnevenRows = true,
				IsPullToRefreshEnabled = true,
				SelectionMode = ListViewSelectionMode.Single,
				SeparatorVisibility = SeparatorVisibility.None,
				ItemsSource = GetListViewItems(),
				ItemTemplate = new DataTemplate(() =>
				{
					var swipeView = new Xamarin.Forms.SwipeView
					{
						Margin = 0,
						Padding = 5,
						BackgroundColor = Color.Gray,
						Content = new Frame
						{
							Padding = 0,
							BackgroundColor = Color.DarkGray,
							Content = new Label
							{
								Text = "Swipe to Left",
								Margin = new Thickness(10, 30)
							}
						}
					};

					swipeView.RightItems.Add(new SwipeItem { Text = "B1", BackgroundColor = Color.Red });
					swipeView.RightItems.Add(new SwipeItem { Text = "B2", BackgroundColor = Color.Blue });
					swipeView.RightItems.Add(new SwipeItem { Text = "B3", BackgroundColor = Color.Green });

					return new ViewCell
					{
						View = new Frame
						{
							Padding = 0,
							Margin = new Thickness(4, 2),
							BorderColor = Color.Transparent,
							BackgroundColor = Color.Transparent,
							Content = swipeView
						}
					};
				})
			};

			var description = new Label
			{
				Padding = 12,
				Text = "Rapidly swipe multiple times, if the SwipeView maintain the same position, the test has passed."
			};

			var content = new StackLayout();

			content.Children.Add(description);
			content.Children.Add(listView);

			Content = content;
		}

		List<string> GetListViewItems()
		{
			return new List<string>
			{
				"Item 1",
				"Item 2",
				"Item 3",
				"Item 4",
				"Item 5",
				"Item 6",
				"Item 7",
				"Item 8",
				"Item 9",
				"Item 10"
			};
		}
	}
}
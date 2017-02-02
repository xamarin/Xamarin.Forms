using System;
using System.Collections.Generic;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Bugzilla, 46632, "[WinRT/UWP] Clicking button in ListView ViewCell triggers both button clicked and cell ItemTapped", PlatformAffected.WinRT)]
	public class Bugzilla46632 : TestContentPage
	{
		protected override void Init()
		{
			var items = new List<string>() { "ONE", "TWO", "THREE", "FOUR", "FIVE", "SIX" };
			var listView = new ListView
			{
				HasUnevenRows = true,
				SeparatorColor = Color.Black,
				ItemsSource = items,
				ItemTemplate = new DataTemplate(typeof(Bugzilla46632Cell))
			};

			listView.ItemTapped += async (sender, args) =>
			{
				listView.IsEnabled = false;

				await Application.Current.MainPage.DisplayAlert("Tap", "ListItem Tapped", "OK");

				listView.SelectedItem = null;
				listView.IsEnabled = true;
			};

			Content = new StackLayout
			{
				Children =
				{
					new Label
					{
						Text = "When clicking/tapping a cell button below, only the alert for the button should pop up. Likewise, clicking/tapping the cell itself should only show the alert for the cell."
					},
					listView
				}
			};
		}
		class Bugzilla46632Cell : ViewCell
		{
			public Bugzilla46632Cell()
			{
				var entry = new Entry
				{
					Placeholder = "Test"
				};
				var button = new Button
				{
					Text = "Click Me!",
					BackgroundColor = Color.Green,
				};
				button.Clicked += async (s, e) => await Application.Current.MainPage.DisplayAlert("Click", "Button Clicked", "OK");
				var label = new Label();
				label.SetBinding(Label.TextProperty, ".");

				View = new StackLayout
				{
					Orientation = StackOrientation.Horizontal,
					Children =
					{
						button,
						label,
						entry
					},
					Padding = 20,
				};
			}
		}
	}
}
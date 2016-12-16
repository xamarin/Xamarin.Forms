using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Bugzilla, 42301, "Grid or ContentPage extends beyond screen bounds after certain navigation types.", PlatformAffected.WinRT)]
	public class Bugzilla42301 : TestNavigationPage
	{
		protected override void Init()
		{
			PushAsync(new TestPage42301());
		}
	}

	[Preserve(AllMembers = true)]
	public class TestPage42301 : ContentPage
	{
		public TestPage42301()
		{
			var button = new Button
			{
				Text = "Click Me"
			};

			button.Clicked += (sender, e) =>
			{
				Navigation.PushModalAsync(new TestPage42301());
			};

			var grid = new Grid
			{
				AutomationId = "Grid",
				BackgroundColor = Color.Fuchsia,
				Padding = 0,
				RowSpacing = 0,
				ColumnSpacing = 0,
				RowDefinitions =
					{
						new RowDefinition {Height = new GridLength(1, GridUnitType.Auto)},
						new RowDefinition {Height = new GridLength(1, GridUnitType.Star)}
					}
			};

			grid.Children.Add(
				new ContentView
				{
					BackgroundColor = Color.Red,
					HeightRequest = 50
				},
				0, 0);

			grid.Children.Add(
				new ContentView
				{
					BackgroundColor = Color.Green,
					Content = button
				},
				0, 1);

			grid.Children.Add(
				new ContentView
				{
					BackgroundColor = Color.Blue,
					HeightRequest = 50,
					VerticalOptions = LayoutOptions.End,
					Content = new StackLayout
					{
						VerticalOptions = LayoutOptions.End,
						Children =
						{
							new Label { Text = "This should remain visible" }
						}
					}
				},
				0, 1);

			Content = grid;
		}
	}
}
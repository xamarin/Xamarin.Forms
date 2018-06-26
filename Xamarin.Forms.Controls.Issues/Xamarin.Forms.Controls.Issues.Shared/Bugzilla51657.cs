using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using System.Linq;

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Bugzilla, 51657, "CarouselView does not reassign ViewPager.Adapter when used within TabbedPage", PlatformAffected.Android)]
	public class Bugzilla51657 : TestTabbedPage
	{
		protected override void Init()
		{
			Children.Add(new TabPage1() { Title = "Tab 1" });
			Children.Add(new ContentPage { BackgroundColor = Color.Teal, Content = new Label { Text = "Tab Page 2", VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Center }, Icon = "coffee.png", Title = "Tab 2" });
			Children.Add(new ContentPage { BackgroundColor = Color.Silver, Content = new Label { Text = "Tab Page 3", VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Center }, Icon = "coffee.png", Title = "Tab 3" });
		}

		class TabPage1 : ContentPage
		{
			public TabPage1()
			{
				var carouselView = new CarouselView
				{
					ItemsSource = Enumerable.Range(0, 10).ToList(),
					ShowIndicators = true,
					ItemTemplate = new DataTemplate(() =>
					{
						var image = new Image { Source = "photo.jpg", Aspect = Aspect.AspectFill };
						var label = new Label
						{
							TextColor = Color.White,
							Text = "Switch to Tab 3, then back to Tab 1. If the background image is no longer visible, this test has failed.",
							FontSize = 16,
							HorizontalOptions = LayoutOptions.Center,
							VerticalOptions = LayoutOptions.CenterAndExpand
						};
						var stack = new StackLayout { BackgroundColor = Color.FromHex("#80000000"), Padding = 12, Children = { label } };

						var grid = new Grid
						{
							RowDefinitions = {
								new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
								new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) }
							}
						};

						Grid.SetRowSpan(image, 2);
						grid.Children.Add(image);
						grid.Children.Add(stack, 0, 1);

						return grid;
					})
				};

				var boxView = new BoxView { BackgroundColor = Color.Blue };

				var layout = new AbsoluteLayout();
				layout.Children.Add(carouselView, new Rectangle(0, 0, 1, 1), AbsoluteLayoutFlags.All);
				layout.Children.Add(boxView, new Rectangle(0.5, 0.5, 100, 100), AbsoluteLayoutFlags.PositionProportional);
				Content = layout;
			}
		}
	}
}
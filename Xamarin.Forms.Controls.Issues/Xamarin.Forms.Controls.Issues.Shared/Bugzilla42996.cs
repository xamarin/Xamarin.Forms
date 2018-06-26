using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using System.Linq;

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Bugzilla, 42996, "CarouselView does not layout children properly on UWP", PlatformAffected.UWP)]
	public class Bugzilla42996 : TestNavigationPage
	{
		protected override void Init()
		{
			var page = new ContentPage();
			var carouselView = new CarouselView
			{
				ItemsSource = Enumerable.Range(0, 10).ToList(),
				ItemTemplate = new DataTemplate(() =>
				{
					var label = new Label { Text = "Blah", BackgroundColor = Color.White, Margin = new Thickness(10) };
					var stack = new StackLayout { Children = { label } };
					return new ScrollView { Content = stack };
				})
			};

			page.Content = carouselView;

			Navigation.PushAsync(page);
		}
	}
}
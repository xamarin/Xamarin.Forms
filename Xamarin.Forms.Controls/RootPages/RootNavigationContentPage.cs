namespace Xamarin.Forms.Controls
{
	// NavigationPage -> ContentPage
	public class RootNavigationContentPage : NavigationPage
	{
		public RootNavigationContentPage(string hierarchy)
		{
			AutomationId = hierarchy + "PageId";

			var content = new ContentPage
			{
				BackgroundColor = Color.Yellow,
				Title = "Testing 123",
				Content = new SwapHierachyStackLayout(hierarchy)
			};

			PushAsync(content);
		}
	}
}
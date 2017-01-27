namespace Xamarin.Forms.Controls
{
	// NavigationPage -> TabbedPage -> ContentPage
	// Not recommended
	public class RootNavigationTabbedContentPage : NavigationPage
	{
		public RootNavigationTabbedContentPage(string hierarchy)
		{
			AutomationId = hierarchy + "PageId";

			var tabbedPage = new TabbedPage
			{
				Children =
				{
					new ContentPage
					{
						Title = "Page 1",
						Content = new SwapHierachyStackLayout(hierarchy)
					},
					new ContentPage
					{
						Title = "Page 2",
						Content = new StackLayout
						{
							Children =
							{
								new Label { Text = "Page Two" },
								new BoxView
								{
									Color = Color.Gray,
									VerticalOptions = LayoutOptions.FillAndExpand,
									HorizontalOptions = LayoutOptions.FillAndExpand
								},
								new Button { Text = "Click me" },
							}
						}
					}
				}
			};

			PushAsync(tabbedPage);
		}
	}
}
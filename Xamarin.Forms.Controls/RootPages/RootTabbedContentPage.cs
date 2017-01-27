namespace Xamarin.Forms.Controls
{
	// TabbedPage -> ContentPage
	public class RootTabbedContentPage : TabbedPage
	{
		public RootTabbedContentPage(string hierarchy)
		{
			AutomationId = hierarchy + "PageId";

			var tabOne = new ContentPage
			{
				Title = "Testing 123",
				Content = new SwapHierachyStackLayout(hierarchy)
			};

			var tabTwo = new ContentPage
			{
				Title = "Testing 345",
				Content = new StackLayout
				{
					Children =
					{
						new Label { Text = "Hello" },
						new AbsoluteLayout
						{
							BackgroundColor = Color.Red,
							VerticalOptions = LayoutOptions.FillAndExpand,
							HorizontalOptions = LayoutOptions.FillAndExpand
						},
						new Button { Text = "Button" },
					}
				}
			};

			Children.Add(tabOne);
			Children.Add(tabTwo);
		}
	}
}
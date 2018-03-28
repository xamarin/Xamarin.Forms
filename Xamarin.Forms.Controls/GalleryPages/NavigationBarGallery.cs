namespace Xamarin.Forms.Controls
{
	public class NavigationBarGallery : ContentPage
	{
		public NavigationBarGallery(NavigationPage rootNavPage)
		{
			int toggleBarTextColor = 0;
			int toggleBarBackgroundColor = 0;

			ToolbarItems.Add(new ToolbarItem { Text = "Save" });

			NavigationPage.SetTitleIcon(this, "coffee.png");

			NavigationPage.SetTitleView(this, CreateTitleView());

			rootNavPage.BarHeight = 450;

			Content = new ScrollView { Content = 
					new StackLayout
					{
						Children = {
						new Button {
							Text = "Change BarTextColor",
							Command = new Command (() => {
								if (toggleBarTextColor % 2 == 0) {
									rootNavPage.BarTextColor = Color.Teal;
								} else {
									rootNavPage.BarTextColor = Color.Default;
								}
								toggleBarTextColor++;
							})
						},
						new Button {
							Text = "Change BarBackgroundColor",
							Command = new Command (() => {
								if (toggleBarBackgroundColor % 2 == 0) {
									rootNavPage.BarBackgroundColor = Color.Navy;
								} else {
									rootNavPage.BarBackgroundColor = Color.Default;
								}
								toggleBarBackgroundColor++;

							})
						},
						new Button {
							Text = "Change Both to default",
							Command = new Command (() => {
								rootNavPage.BarTextColor = Color.Default;
								rootNavPage.BarBackgroundColor = Color.Default;
							})
						},
						new Button {
							Text = "Make sure Tint still works",
							Command = new Command (() => {
	#pragma warning disable 618
								rootNavPage.Tint = Color.Red;
	#pragma warning restore 618
							})
						},
						new Button {
							Text = "Black background, white text",
							Command = new Command (() => {
								rootNavPage.BarTextColor = Color.White;
								rootNavPage.BarBackgroundColor = Color.Black;
							})
						},
						new Button {
							Text = "Toggle TitleIcon",
							Command = new Command (() => {

								var titleIcon = NavigationPage.GetTitleIcon(this);

								if (titleIcon == null)
									titleIcon = "coffee.png";
								else
									titleIcon = null;

								NavigationPage.SetTitleIcon(this, titleIcon);
							})
						},
						new Button {
							Text = "Toggle TitleView",
							Command = new Command (() => {

								var titleView = NavigationPage.GetTitleView(this);

								if (titleView == null)
									titleView = CreateTitleView();
								else
									titleView = null;

								NavigationPage.SetTitleView(this, titleView);
							})
						},
						new Button {
							Text = "Toggle Back Title",
							Command = new Command (() => {

								var backTitle = NavigationPage.GetBackButtonTitle(rootNavPage);

								if (backTitle == null)
									backTitle= "Go back home";
								else
									backTitle = null;

								NavigationPage.SetBackButtonTitle(rootNavPage, backTitle);
							})
						},
						new Button {
							Text = "Toggle Toolbar Item",
							Command = new Command (() => {

								if (ToolbarItems.Count > 0)
									ToolbarItems.Clear();
								else
									ToolbarItems.Add(new ToolbarItem { Text = "Save" });
							})
						},
						new Button {
							Text = "Toggle Title",
							Command = new Command (() => {

								if (Title == null)
									Title = "NavigationBar Gallery - Legacy";
								else
									Title = null;
							})
						},
						new Button {
							Text = "Toggle BarHeight",
							Command = new Command (() => {

								if (rootNavPage.BarHeight == -1)
									rootNavPage.BarHeight= 450;
								else
									rootNavPage.ClearValue(NavigationPage.BarHeightProperty);
							})
						}
					}
				}
			};
		}

		static VisualElement CreateTitleView()
		{
			var titleView = new StackLayout
			{
				Children = {
						new Label { Text = "TitleView", FontSize = 20, HorizontalTextAlignment = TextAlignment.Center },
						new Label { Text = "it's lovely", HorizontalTextAlignment = TextAlignment.Center },
						new SearchBar { VerticalOptions = LayoutOptions.Center }
				},
				Spacing = 0,
				BackgroundColor = Color.Pink,
				Margin = new Thickness(50, 0),
				HorizontalOptions = LayoutOptions.Fill
			};
			return titleView;
		}
	}
}
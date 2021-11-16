using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 14763, "[iOS] Resize on back-swipe stays when going back is cancelled", PlatformAffected.iOS)]
	public class Issue14763 : NavigationPage
	{
		public Issue14763() : base(new HomePage())
		{ }

		class HomePage : ContentPage
		{
			public HomePage()
			{
				Title = "Home";
				var grd = new Grid { BackgroundColor = Color.Brown };
				grd.RowDefinitions.Add(new RowDefinition());
				grd.RowDefinitions.Add(new RowDefinition());
				grd.RowDefinitions.Add(new RowDefinition());

				NavigationPage.SetHasNavigationBar(this, false);

				var boxView = new BoxView { BackgroundColor = Color.Blue };
				grd.Children.Add(boxView, 0, 0);
				var stackLayout = new StackLayout()
				{
					BackgroundColor = Color.Yellow
				};
				var btnPop = new Button {
					Text = "Back (I get cut off)", AutomationId = "PopButtonId", Command = new Command(async () => await Navigation.PopAsync()),
					BackgroundColor = Color.Red
				};
				stackLayout.Children.Add(btnPop);
				btnPop.VerticalOptions = new LayoutOptions(LayoutAlignment.End, expands: true);
				var btn = new Button()
				{
					BackgroundColor = Color.Pink,
					Text = "NextButtonID",
					AutomationId = "NextButtonID",
					
					Command = new Command(async () =>
					{
						
						var page = new ContentPage
						{
							Title = "Detail",
							Content = stackLayout
						};

						NavigationPage.SetHasNavigationBar(page, true);
						await Navigation.PushAsync(page);
					})
				};

				grd.Children.Add(btn, 0, 1);
				var image = new Image() { Source = "coffee.png", AutomationId = "CoffeeImageId", BackgroundColor = Color.Yellow };
				image.VerticalOptions = LayoutOptions.End;
				grd.Children.Add(image, 0, 2);
				Content = grd;

			}
		}
	}
}
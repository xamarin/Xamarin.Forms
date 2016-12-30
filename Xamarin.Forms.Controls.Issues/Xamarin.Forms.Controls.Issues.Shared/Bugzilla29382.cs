using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using Xamarin.Forms.PlatformConfiguration;
#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Bugzilla, 29382, "PushModalAsync without animations before showing first page still shows original page")]
	public class Bugzilla29382 : TestContentPage
	{
		//public App()
		//{
		//	InitializeComponent();

		//	Xamarin.Forms.PlatformConfiguration.iOSSpecific.Application.SetPageOverlayColorWhenPushingModal(Current.On<iOS>(), Color.Green);

		//	//MainPage = new Page1();
		//	//MainPage = new NavigationPage(new Page1());
		//	MainPage = new TabbedPage
		//	{
		//		Children = { new Page1 { Title = "Tab1" }, new Page1 { Title = "Tab2" }, new Page1 { Title = "Tab3" }, new Page1 { Title = "Tab4" }, new Page1 { Title = "Tab5" } }
		//	};
		//	MainPage.Navigation.PushModalAsync(new Modal(false), false);
		//}

		protected override void Init()
		{
			var contentView = new ContentView();
			var stackLayout = new StackLayout
			{
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center,
				Spacing = 20,
				Children =
				{
					new Button
					{
						Text = "Push animated",
						HorizontalOptions = LayoutOptions.Center,
						VerticalOptions = LayoutOptions.Center,
						BackgroundColor = Color.Black,
						TextColor = Color.White,
						WidthRequest = 250,
						HeightRequest = 50,
						Command = new Command(() => Navigation.PushModalAsync(new Modal(true), true))
					},
					new Button
					{
						Text = "Push non-animated",
						HorizontalOptions = LayoutOptions.Center,
						VerticalOptions = LayoutOptions.Center,
						BackgroundColor = Color.Black,
						TextColor = Color.White,
						WidthRequest = 250,
						HeightRequest = 50,
						Command = new Command(() => Navigation.PushModalAsync(new Modal(false), false))
					}
				}
			};
			contentView.Content = stackLayout;
			Content = contentView;
		}

		public class Modal : ContentPage
		{
			public Modal(bool animated)
			{
				BackgroundColor = Color.Green;

				var button = new Button
				{
					Text = "Pop " + (animated ? "animated" : "non-animated"),
					HorizontalOptions = LayoutOptions.Center,
					VerticalOptions = LayoutOptions.Center,
					BackgroundColor = Color.Black,
					TextColor = Color.White,
					WidthRequest = 250,
					HeightRequest = 50,
					Command = new Command(() => Navigation.PopModalAsync(animated))
				};

				var grid = new Grid
				{
					Children = { button }
				};
				Content = grid;
			}

			//protected override void OnAppearing()
			//{
			//	base.OnAppearing();

			//	Xamarin.Forms.PlatformConfiguration.iOSSpecific.Application.DisablePageOverlayColorWhenPushingModal(Current.On<iOS>());
			//}
		}
	}
}
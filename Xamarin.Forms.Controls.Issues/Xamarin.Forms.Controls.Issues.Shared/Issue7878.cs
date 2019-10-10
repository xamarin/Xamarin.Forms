using System;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 7878, "Page not popped on iOS 13 FormSheet swipe down", PlatformAffected.iOS)]
	public class Issue7878 : TestContentPage
	{
		ContentPage _modalPage;

		protected override void Init()
		{
			var modalButton = new Button
			{
				Text = "Modal me",
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center
			};

			modalButton.Clicked += Button_Clicked;

			Content = modalButton;

			_modalPage = new ContentPage();

			var button = new Button
			{
				Text = "Pressing this raises the popped event, swiping down doesn't",
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center
			};

			button.Clicked += (o, a) =>
			{
				Navigation.PopModalAsync();
			};

			_modalPage.Content = button;
		}

		void Button_Clicked(System.Object sender, System.EventArgs e)
		{
			var navigationPage = new NavigationPage(_modalPage);
			Xamarin.Forms.PlatformConfiguration.iOSSpecific.Page.SetModalPresentationStyle(navigationPage.On<Xamarin.Forms.PlatformConfiguration.iOS>(), Xamarin.Forms.PlatformConfiguration.iOSSpecific.UIModalPresentationStyle.FormSheet);
			Navigation.PushModalAsync(navigationPage);
		}
	}
}

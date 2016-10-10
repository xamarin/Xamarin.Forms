using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;
#if UITEST
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.None, 0, "Page twitching when toolbar is hidden in page", PlatformAffected.All, NavigationBehavior.PushAsync)]
	class TollbarHideAppCompat : TestContentPage
	{
		public TollbarHideAppCompat()
		{
			NavigationPage.SetHasNavigationBar(this, false);
		}

		protected override void Init()
		{
			Title = "Hidden toolbar";

			BackgroundColor = Color.Red;

			Content = new ContentView()
			{
				Margin = new Thickness(5),
				BackgroundColor = Color.White,
				Content = new Label()
				{
					VerticalOptions = LayoutOptions.Center,
					HorizontalOptions = LayoutOptions.Center,
					Text = "Text"
				}
			};

			Pr431Test();
		}

		public async void Pr431Test()
		{
			await Task.Delay(2000);
			await Navigation.PushAsync(new ContentPage());
			await Task.Delay(2000);
			await Navigation.PopAsync();
			await Task.Delay(2000);
			NavigationPage.SetHasNavigationBar(this, true);
			await Task.Delay(2000);

		}
	}
}

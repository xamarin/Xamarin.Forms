using System;
using System.Threading.Tasks;

namespace Xamarin.Forms.Controls
{
	public class Bugzilla44596SplashPage : ContentPage
	{
		public Bugzilla44596SplashPage(Action finishedLoading)
		{
			BackgroundColor = Color.Blue;
			FinishedLoading = finishedLoading;
		}

		Action FinishedLoading { get; set; }

		protected async override void OnAppearing()
		{
			base.OnAppearing();
			await Task.Delay(2000);
			FinishedLoading?.Invoke();
		}
	}
}
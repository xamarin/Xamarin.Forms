using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace bvbbf
{
	public partial class MainPage : ContentPage
	{
		public MainPage()
		{
			InitializeComponent();
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();

			
		}

		void Button_OnClicked(object sender, EventArgs e)
		{
			(App.Current.MainPage as NavigationPage).Navigation.PushAsync(new ContentPage());
		}
	}
}

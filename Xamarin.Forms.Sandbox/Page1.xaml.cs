using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Xamarin.Forms.Sandbox
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class Page1 : ContentPage
	{
		public Page1()
		{
			InitializeComponent();
		}

		private async void Button_Clicked(object sender, EventArgs e)
		{

			//await Navigation.PushAsync(new EditPage());
			await Shell.Current.CurrentItem.CurrentItem.GoToAsync(new List<string> { "edit" }, new Dictionary<string, string>(), false);
		}
	}
}
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
	public partial class Page2 : ContentPage
	{
		public Page2()
		{
			InitializeComponent();
		}

		private async void justshellcontent_Clicked(object sender, EventArgs e)
		{
			await Shell.Current.GoToAsync("//justshellcontent");
		}

		private async void justshellcontentAndEdit_Clicked(object sender, EventArgs e)
		{
			await Shell.Current.GoToAsync("//justshellcontent/edit");
		}
	}
}
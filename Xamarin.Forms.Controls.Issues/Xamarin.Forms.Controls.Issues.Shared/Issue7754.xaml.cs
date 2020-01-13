using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Xamarin.Forms.Controls.Issues
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class Issue7754 : ContentPage
	{
		public Issue7754()
		{
			InitializeComponent();
			btn.Clicked += Btn_Clicked;
		}

		Color color = Color.Blue;
		void Btn_Clicked(object sender, EventArgs e)
		{
			color = (color == Color.Blue) ? Color.Fuchsia : Color.Blue;

			NavigationPage.SetBackgroundTitleView(this, color);
		}
	}
}
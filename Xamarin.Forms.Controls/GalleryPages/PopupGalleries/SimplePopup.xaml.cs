using Xamarin.Forms.Xaml;

namespace Xamarin.Forms.Controls
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SimplePopup : Popup
	{
		public SimplePopup()
		{
			InitializeComponent();
		}

		private void Button_Clicked(object sender, System.EventArgs e)
		{
			Dismiss(null);
		}
	}
}
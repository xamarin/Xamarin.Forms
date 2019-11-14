using Xamarin.Forms.Controls;
using Xamarin.Forms.Platform.WPF;

namespace Xamarin.Forms.ControlGallery.WPF
{
	/// <summary>
	/// Logique d'interaction pour MainWindow.xaml
	/// </summary>
	public partial class MainWindow : FormsApplicationPage
	{
		public MainWindow()
		{
			InitializeComponent();
			
			LoadApplication(Xamarin.Forms.Forms.Create()
				.WithMaps("")
				.UseStartup<Startup>()
				.Build<Controls.App>()
			);
		}
	}
}

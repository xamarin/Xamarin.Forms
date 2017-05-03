using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xamarin.Forms;
using Xamarin.Forms.ControlGallery.WPF;
using Xamarin.Forms.Controls;

[assembly: Dependency(typeof(StringProvider))]
namespace Xamarin.Forms.ControlGallery.WPF
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow 
	{
		public MainWindow()
		{
			Forms.Init();
			var app=new Xamarin.Forms.Controls.App();
			InitializeComponent();
			LoadApplication(app);
		}
	}

	public class StringProvider : IStringProvider
	{
		public string CoreGalleryTitle { get { return "WP8 Core Gallery"; } }
	}
}

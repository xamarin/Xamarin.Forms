using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.ControlGallery.iOS;
using Xamarin.Forms.Controls.Issues;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(Issue10337NavigationPage), typeof(_10337CustomRenderer))]
namespace Xamarin.Forms.ControlGallery.iOS
{
    public class _10337CustomRenderer : NavigationRenderer
    {
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var isiOS13OrNewer = UIDevice.CurrentDevice.CheckSystemVersion(13, 0);

            if (isiOS13OrNewer)
            {
                var navigationBarAppearance = NavigationBar.StandardAppearance;
                navigationBarAppearance.ConfigureWithDefaultBackground();
                navigationBarAppearance.BackgroundColor = UIColor.Red;
                navigationBarAppearance.ShadowImage = new UIImage();
                navigationBarAppearance.ShadowColor = UIColor.Clear;
                NavigationBar.StandardAppearance = navigationBarAppearance;
                System.Diagnostics.Debug.WriteLine("_10337CustomRenderer");
            }
			else
			{
                NavigationBar.ShadowImage = new UIImage();
            }
        }
    }
}
using UIKit;
using Xamarin.Forms.Platform.iOS;

namespace Xamarin.Forms.ControlGallery.iOS.CustomRenderers
{
	public class DetailPageStatusBarRenderer : NavigationRenderer
	{
		public override UIStatusBarStyle PreferredStatusBarStyle()
		{
			return UIStatusBarStyle.DarkContent;
		}
	}

	public class FlyoutPageStatusRenderer : PageRenderer
	{
		public override UIStatusBarStyle PreferredStatusBarStyle()
		{
			return UIStatusBarStyle.LightContent;
		}
	}

}

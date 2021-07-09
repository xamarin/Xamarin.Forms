using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.ControlGallery.iOS.CustomRenderers;
using Xamarin.Forms.Controls.XamStore;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(DemoShellPage), typeof(_DemoShellPageCustomRenderer))]
namespace Xamarin.Forms.ControlGallery.iOS.CustomRenderers
{
	public class _DemoShellPageCustomRenderer : PageRenderer
	{
		protected override void OnElementChanged(VisualElementChangedEventArgs e)
		{
			base.OnElementChanged(e);

			System.Diagnostics.Debug.WriteLine($"{e.NewElement.GetType()} is replaced by _DemoShellPageCustomRenderer");
		}

		public override UIStatusBarStyle PreferredStatusBarStyle()
		{
			return UIStatusBarStyle.LightContent;
		}
	}
}

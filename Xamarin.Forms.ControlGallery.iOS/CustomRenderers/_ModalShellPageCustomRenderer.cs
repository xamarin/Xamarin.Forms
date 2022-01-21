using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.ControlGallery.iOS.CustomRenderers;
using Xamarin.Forms.Controls.XamStore.Views;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ModalShellPage), typeof(_ModalShellPageCustomRenderer))]
namespace Xamarin.Forms.ControlGallery.iOS.CustomRenderers
{
	public class _ModalShellPageCustomRenderer : PageRenderer
	{
		protected override void OnElementChanged(VisualElementChangedEventArgs e)
		{
			base.OnElementChanged(e);

			System.Diagnostics.Debug.WriteLine($"{e.NewElement.GetType()} is replaced by _ModalShellPageCustomRenderer");
		}

		public override UIStatusBarStyle PreferredStatusBarStyle()
		{
			return UIStatusBarStyle.LightContent;
		}
	}
}

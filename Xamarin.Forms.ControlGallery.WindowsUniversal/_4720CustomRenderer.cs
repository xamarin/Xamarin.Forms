using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.ControlGallery.WindowsUniversal;
using Xamarin.Forms.Platform.UWP;

[assembly: ExportRenderer(typeof(WebView), typeof(_4720CustomRenderer))]
namespace Xamarin.Forms.ControlGallery.WindowsUniversal
{
	public class _4720CustomRenderer : WebViewRenderer
	{
		public _4720CustomRenderer()
		{
			base.ExecutionMode = Windows.UI.Xaml.Controls.WebViewExecutionMode.SeparateProcess;
		}
		protected override void OnElementChanged(ElementChangedEventArgs<WebView> e)
		{
			base.OnElementChanged(e);

			if (e.NewElement != null)
				System.Diagnostics.Debug.WriteLine($"{e.NewElement.GetType()} renderer is replaced by _4720CustomRenderer");
		}
	}
}

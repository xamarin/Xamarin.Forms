using Android.Content;
using Xamarin.Forms;
using Xamarin.Forms.ControlGallery.Android;
using Xamarin.Forms.Controls.Issues;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(_8954WebView), typeof(_8954CustomRenderer))]
namespace Xamarin.Forms.ControlGallery.Android
{
	public class _8954CustomRenderer : WebViewRenderer
	{
		private readonly Context _context;
		public _8954CustomRenderer(Context context) : base(context)
		{
			_context = context;
		}

		protected override void OnElementChanged(ElementChangedEventArgs<WebView> e)
		{
			base.OnElementChanged(e);

			if (e.NewElement != null)
			{
				Control.Settings.UserAgentString = "Custom user agent string";
			}
		}
	}
}